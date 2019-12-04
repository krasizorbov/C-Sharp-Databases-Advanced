using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public int Id { get; set; }
        [RegularExpression(@"^[0-9]{4}\s[0-9]{4}\s[0-9]{4}\s[0-9]{4}$")]
        public string Number { get; set; }
        [RegularExpression("^[0-9]{3}$")]
        public string Cvc { get; set; }
        public CardType Type { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
 
    }
}
