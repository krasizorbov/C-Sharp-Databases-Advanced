using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportGameDTO
    {
        [Required]
        public string Name { get; set; }
        [Range(0.00, double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        public string ReleaseDate { get; set; }
        [Required]
        public string Developer { get; set; }
        [Required]
        public string Genre { get; set; }
        public List<string> Tags { get; set; }
    
    }
}
