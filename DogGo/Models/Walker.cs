using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter your Name..")]
        [MaxLength(60)]

        public string Name { get; set; }
        [DisplayName("Image Url")]
        public string ImageUrl { get; set; }
        [Required]
        [DisplayName("Neighborhood")]
        public int NeighborhoodId { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public Neighborhood Neighborhood { get; set; }
    }
}
