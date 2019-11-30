using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType("Prisoner")]
    public class ExportPrisonersDTO
    {
        [XmlElement("Id")]
        public int Id { get; set; }
        [XmlElement("Name")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; }
        [XmlElement("IncarcerationDate")]
        [Required]
        public string IncarcerationDate { get; set; }
        [XmlArray("EncryptedMessages")]
        public EncryptedMessagesDTO[] Message { get; set; }
    }
    [XmlType("Message")]
    public class EncryptedMessagesDTO
    {
        [XmlElement("Description")]
        public string Description { get; set; }
    }
}
