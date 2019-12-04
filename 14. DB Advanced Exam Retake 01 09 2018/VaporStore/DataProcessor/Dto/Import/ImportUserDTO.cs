using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUserDTO
    {
        [Required]
        [RegularExpression("^[A-Z][a-z]+ [A-Z][a-z]+$")]
        public string FullName { get; set; }
        [Required]
        [MinLength(3), MaxLength(20)]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Range(3, 103)]
        public int Age { get; set; }
        public CardDTO[] Cards { get; set; }


    }

    public class CardDTO
    {
        [Required]
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Number { get; set; }
        [Required]
        [RegularExpression("^[0-9]{3}$")]
        public string CVC { get; set; }
        [Required]
        public CardType Type { get; set; }
    }
}
