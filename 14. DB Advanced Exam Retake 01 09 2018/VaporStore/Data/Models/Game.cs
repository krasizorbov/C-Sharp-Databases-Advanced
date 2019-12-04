using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VaporStore.Data.Models
{
	public class Game
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(0.00, double.MaxValue)]
        public decimal Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DeveloperId { get; set; }
        public Developer Developer { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
        public ICollection<GameTag> GameTags { get; set; } = new HashSet<GameTag>();

    }
}
