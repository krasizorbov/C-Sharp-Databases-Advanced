using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Positions
{
    public class PositionsAllViewModel
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
    }
}
