using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("customer")]
    public class ExportCustomersDTO
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; }
        [XmlAttribute("bought-cars")]
        public int Count { get; set; }
        [XmlAttribute("spent-money")]
        public decimal Price { get; set; }
    }
}
