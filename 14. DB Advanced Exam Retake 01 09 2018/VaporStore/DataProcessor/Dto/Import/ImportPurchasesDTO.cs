using System;
using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class ImportPurchasesDTO
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlElement("Type")]
        public PurchaseType Type { get; set; }
        [XmlElement("Key")]
        [Required]
        [RegularExpression("^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string Key { get; set; }
        [XmlElement("Card")]
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Card { get; set; }
        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
