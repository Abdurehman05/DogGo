using DogGo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models.ViewModels
{
    public class WalkerProfileViewModel
    {
        public Walker Walker { get; set; }
        public List<Walker> Walkers { get; set; }
        public List<Walks> Walks { get; set; }
        public List<Dog> Dogs { get; set; } 
        public List<Owner> Owners { get; set; }
        public List<Object> WalkerWalks { get; set; }
        public int TotalWalks;
       

    }
}
