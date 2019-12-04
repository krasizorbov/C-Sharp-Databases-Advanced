using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Performer")]
    public class ImportSongPerformersDTO
    {
        [XmlElement("FirstName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string FirstName { get; set; }
        [XmlElement("LastName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string LastName { get; set; }
        [XmlElement("Age")]
        [Range(18, 70)]
        public int Age { get; set; }
        [XmlElement("NetWorth")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal NetWorth { get; set; }
        [XmlArray("PerformersSongs")]
        public PerformerSongDTO[] PerformersSongs { get; set; }
    }
    [XmlType("Song")]
    public class PerformerSongDTO
    {
        [XmlAttribute("id")]
        public int SongId { get; set; }
    }
}
