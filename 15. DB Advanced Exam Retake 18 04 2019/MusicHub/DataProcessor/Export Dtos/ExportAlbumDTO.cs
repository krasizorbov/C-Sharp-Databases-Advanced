using MusicHub.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.DataProcessor.Export_Dtos
{
    public class ExportAlbumDTO
    {
        [Required]
        [MinLength(3), MaxLength(40)]
        public string Name { get; set; }
        [Required]
        public string ReleaseDate { get; set; }
        public string ProducerName { get; set; } 
        public SongDTO[] Songs { get; set; }
        public decimal AlbumPrice { get; set; }
    }

    public class SongDTO
    {
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; }
        [Required]
        public string Writer { get; set; }
        public decimal Price { get; set; }
    }
}
