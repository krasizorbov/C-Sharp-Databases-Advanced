using Newtonsoft.Json;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.Export
{
    public class UserDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(3)]
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("soldProducts")]
        public ICollection<Product> ProductsSold { get; set; }
    }
}
