using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int ProjectionId { get; set; }
        public Projection Projection { get; set; }
    }
}
