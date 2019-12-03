using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class ImportCustomerTicketDTO
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
        [Range(12, 110)]
        public int Age { get; set; }
        [XmlElement("Balance")]
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Balance { get; set; }
        [XmlArray("Tickets")]
        public TicketDTO[] Tickets { get; set; }
    }
    [XmlType("Ticket")]
    public class TicketDTO
    {
        [XmlElement("ProjectionId")]
        public int ProjectionId { get; set; }
        [XmlElement("Price")]
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
