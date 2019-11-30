using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonersDTO
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string FullName { get; set; }
        [Required]
        [RegularExpression("^The [A-Z][a-z]+$")]
        public string Nickname { get; set; }
        [Required]
        [Range(18, 65)]
        public int Age { get; set; }
        [Required]
        public string IncarcerationDate { get; set; }
        public string ReleaseDate { get; set; }
          
        [Range(0.00, double.MaxValue)]
        public decimal? Bail { get; set; }
        public int? CellId { get; set; }
        public List<ImportMailsDTO> Mails { get; set; }
    }

    public class ImportMailsDTO
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Sender { get; set; }
        [Required]
        [RegularExpression(@"^[\w ]+ str\.$")]
        public string Address { get; set; }
    }
}
