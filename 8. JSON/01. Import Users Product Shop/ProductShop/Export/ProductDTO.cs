using Newtonsoft.Json;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.Export
{
    public class ProductDTO
    {
        [Required]
        [MinLength(3)]
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("seller")]
        public string Seller { get; set; }
    }
}
