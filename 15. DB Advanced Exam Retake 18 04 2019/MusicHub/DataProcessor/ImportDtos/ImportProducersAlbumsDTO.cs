using MusicHub.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.DataProcessor.ImportDtos
{
    public class ImportProducersAlbumsDTO
    {
        [Required]
        [MinLength(3), MaxLength(30)]
        public string Name { get; set; }
        [RegularExpression("[A-Z][a-z]+ [A-Z][a-z]+")]
        public string Pseudonym { get; set; }
        [RegularExpression(@"\+359 [0-9]{3} [0-9]{3} [0-9]{3}")]
        public string PhoneNumber { get; set; }

        public AlbumDTO[] Albums { get; set; }
    }

    public class AlbumDTO
    {
        [Required]
        [MinLength(3), MaxLength(40)]
        public string Name { get; set; }
        [Required]
        public string ReleaseDate { get; set; }
    }
}
