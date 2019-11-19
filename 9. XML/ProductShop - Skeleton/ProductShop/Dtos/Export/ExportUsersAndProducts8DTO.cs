using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class ExportUsersAndProducts8DTO
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("users")]
        public ExportUsersAndProducts88DTO[] Users { get; set; }
    }
    [XmlType("User")]
    public class ExportUsersAndProducts88DTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }
        [XmlElement("lastName")]
        public string LastName { get; set; }
        [XmlElement("age")]
        public int? Age { get; set; }
        [XmlElement("SoldProducts")]
        public SoldProductsDTO SoldProductsDTO { get; set; }
    }

    public class SoldProductsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlArray("products")]
        public ProductDTO[] ProductDTO { get; set; }
    }
}
