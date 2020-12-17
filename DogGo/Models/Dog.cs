using DogGo.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Dog
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter your dog Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [DisplayName("Owner")]
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
        [Required]
        [StringLength(55,MinimumLength =6)]
        public string Breed { get; set; }
        [StringLength(300)]
        public string Notes { get; set; }
        public string ImageUrl { get; set; }
    }
}
