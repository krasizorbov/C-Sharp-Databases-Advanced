namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentDto = JsonConvert.DeserializeObject<Department[]>(jsonString);

            var sb = new StringBuilder();
            var validDepartments = new List<Department>();

            foreach (var depDto in departmentDto)
            {
                if (!IsValid(depDto) || !depDto.Cells.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                validDepartments.Add(depDto);

                sb.AppendLine($"Imported {depDto.Name} with {depDto.Cells.Count} cells");
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonerDTO = JsonConvert.DeserializeObject<ImportPrisonersDTO[]>(jsonString);

            var sb = new StringBuilder();
            var validPrisoners = new List<Prisoner>();

            foreach (var prisoner in prisonerDTO)
            {
                if (!IsValid(prisoner) || !prisoner.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var p = new Prisoner
                {
                    FullName = prisoner.FullName,
                    Nickname = prisoner.Nickname,
                    Age = prisoner.Age,
                    IncarcerationDate = DateTime.ParseExact(prisoner.IncarcerationDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReleaseDate = prisoner.ReleaseDate == null ? new DateTime?() : DateTime.ParseExact(prisoner.ReleaseDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Bail = prisoner.Bail,
                    CellId = prisoner.CellId,
                    Mails = prisoner.Mails.Select(m => new Mail
                    {
                        Description = m.Description,
                        Sender = m.Sender,
                        Address = m.Address
                    }).ToArray()
                };

                validPrisoners.Add(p);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportOfficersDTO[]), new XmlRootAttribute("Officers"));

            var officersDTO = (ImportOfficersDTO[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validOfficers = new List<Officer>();

            var sb = new StringBuilder();

            foreach (var officer in officersDTO)
            {
                var isValidPosition = Enum.TryParse(officer.Position, out Position resultPosition);
                var isValidWeapon = Enum.TryParse(officer.Weapon, out Weapon resultWeapon);

                if (!IsValid(officer) || !isValidPosition || !isValidWeapon)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var o = new Officer
                {
                    FullName = officer.FullName,
                    Salary = officer.Salary,
                    Position = resultPosition,
                    Weapon = resultWeapon,
                    DepartmentId = officer.DepartmentId,
                    OfficerPrisoners = officer.Prisoners.Select(s => new OfficerPrisoner 
                    {
                        PrisonerId = s.Id
                    }).ToArray()
                };
                validOfficers.Add(o);
                sb.AppendLine($"Imported {officer.FullName} ({officer.Prisoners.Count()} prisoners)");

            }

            context.Officers.AddRange(validOfficers);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            var result = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return result;
        }
    }
}