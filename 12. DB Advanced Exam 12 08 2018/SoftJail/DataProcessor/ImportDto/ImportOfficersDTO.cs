using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficersDTO
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(3), MaxLength(30)]
        public string FullName { get; set; }
        [XmlElement("Money")]
        [Required]
        [Range(0.00, double.MaxValue)]
        public decimal Salary { get; set; }
        [XmlElement("Position")]
        [Required]
        public string Position { get; set; }
        [XmlElement("Weapon")]
        [Required]
        public string Weapon { get; set; }
        [XmlElement("DepartmentId")]
        [Required]
        public int DepartmentId { get; set; }
        [XmlArray("Prisoners")]
        public PrisonersDTO[] Prisoners { get; set; }
    }
    [XmlType("Prisoner")]
    public class PrisonersDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
