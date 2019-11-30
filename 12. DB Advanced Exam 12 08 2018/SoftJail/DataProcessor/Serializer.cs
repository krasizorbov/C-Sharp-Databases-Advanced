namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners.Where(p => ids.Contains(p.Id)).Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(s => new 
                    {
                        OfficerName = s.Officer.FullName,
                        Department = s.Officer.Department.Name
                    })
                    .OrderBy(g => g.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers.Select(f => f.Officer.Salary).Sum()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            return JsonConvert.SerializeObject(prisoners, Newtonsoft.Json.Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisoners = context.Prisoners
                .Where(p => prisonersNames.Contains(p.FullName))
                .Select(x => new ExportPrisonersDTO
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    Message = x.Mails.Select(s => new EncryptedMessagesDTO
                    {
                        Description = ReverseString(s.Description)
                    })
                    .ToArray()
                })
                .OrderBy(f => f.Name)
                .ThenBy(d => d.Id)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportPrisonersDTO[]), new XmlRootAttribute("Prisoners"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });
            xmlSerializer.Serialize(new StringWriter(sb), prisoners, namespaces);

            return sb.ToString().TrimEnd();
        }

        private static string ReverseString(string description)
        {
            var result = string.Join("", description.Reverse());
            return result.ToString();
        }
    }
}