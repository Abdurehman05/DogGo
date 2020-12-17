using DogGo.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DogGo.Repositories
{
    public class Owner
    {
        public int Id { get; set; }
       [EmailAddress]
       [Required]
        public string Email { get; set; }
        [Required(ErrorMessage = "Dude how you dare miss your Name..")]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [StringLength(50,MinimumLength =6)]
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }
        [Required]
        [DisplayName("Neighborhood")]
        public int NeighborhoodId { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public List<Dog> Dogs { get; set; }
    }
}