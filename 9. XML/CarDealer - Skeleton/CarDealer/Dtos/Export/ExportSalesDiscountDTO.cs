using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("sale")]
    public class ExportSalesDiscountDTO
    {
        [XmlElement("car")]
        public ExportCarsDTO Car { get; set; }
        [XmlElement("discount")]
        public string Discount { get; set; }
        [XmlElement("customer-name")]
        public string Name { get; set; }
        [XmlElement("price")]
        public string Price { get; set; }
        [XmlElement("price-with-discount")]
        public string PriceDiscount { get; set; }
    }
}
