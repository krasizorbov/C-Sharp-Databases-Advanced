using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public PurchaseType Type { get; set; }
        [RegularExpression("^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string ProductKey { get; set; }
        public DateTime Date { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }

    }
}
