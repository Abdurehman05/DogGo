using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class OwnersController : Controller
    {
        private readonly IOwnerRepository _ownerRepo;
        private readonly IDogRepository _dogRepository;
        private readonly IWalkerRepository _walkerRepository;
        private readonly INeighborhoodRepository _neighborhoodRepo;
        public OwnersController(IOwnerRepository ownerRepository, IDogRepository dogRepository, IWalkerRepository walkerRepository, INeighborhoodRepository neighborhoodRepo)
        {
            _ownerRepo = ownerRepository;
            _dogRepository = dogRepository;
            _walkerRepository = walkerRepository;
            _neighborhoodRepo = neighborhoodRepo;
        }
        // GET: OwnersController
        public ActionResult Index()
        {
            int currentUserId = GetCurrentUserId();
            return RedirectToAction("Details", new { id = currentUserId });
        }

        // GET: OwnersController/Details/5
        public ActionResult Details(int id)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != id)
            {
                return NotFound();
            }
            Owner owner = _ownerRepo.GetOwnerById(id);
            List<Dog> dogs = _dogRepository.GetDogsByOwnerId(owner.Id);
            List<Walker> walkers = _walkerRepository.GetWalkersInNeighborhood(owner.NeighborhoodId);
            

            OwnerProfileViewModel vm = new OwnerProfileViewModel()
            {
                Owner = owner,
                Dogs = dogs,
                Walkers = walkers
            };
            if(owner == null)
            {
                return NotFound();
            }
            
            return View(vm);
        }

        // GET: OwnersController/Create
        public ActionResult Create()
        {
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();
            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };
            return View(vm);
        }

        // POST: OwnersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OwnerFormViewModel viewModel)
        {
            try
            {
                viewModel.ErrorMessage = "Woops! Something went wrong while saving this owner";
                _ownerRepo.AddOwner(viewModel.Owner);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                viewModel.Neighborhoods = _neighborhoodRepo.GetAll();
                return View(viewModel);
            }
        }

        // GET: OwnersController/Edit/5
        public ActionResult Edit(int id)
        {
            int currentUserId = GetCurrentUserId();
            if(currentUserId != id)
            {
                return NotFound();
            }
            List<Neighborhood> neighborhoods = _neighborhoodRepo.GetAll();
            OwnerFormViewModel vm = new OwnerFormViewModel()
            {
                Owner = new Owner(),
                Neighborhoods = neighborhoods
            };
            return View(vm);
        }

        // POST: OwnersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, OwnerFormViewModel viewModel)
        {
            try
            {
                viewModel.ErrorMessage = "Woops! Something went wrong while saving this owner";
                _ownerRepo.UpdateOwner(viewModel.Owner);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                viewModel.Neighborhoods = _neighborhoodRepo.GetAll();
                return View(viewModel);
            }
        }

        // GET: OwnersController/Delete/5
        public ActionResult Delete(int id)
        {
            Owner owner = _ownerRepo.GetOwnerById(id);
            if(owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: OwnersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Owner owner)
        {
            try
            {
                _ownerRepo.DeleteOwner(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            Owner owner = _ownerRepo.GetOwnerByEmail(viewModel.Email);

            if (owner == null)
            {
                return Unauthorized();
            }

            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
        new Claim(ClaimTypes.Email, owner.Email),
        new Claim(ClaimTypes.Role, "DogOwner"),
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Dogs");
        }
         private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
