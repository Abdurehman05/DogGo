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
    public class WalkersController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalksRepository _walksRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly IDogRepository _dogRepo;

        public WalkersController(IWalkerRepository walkerRepo, IWalksRepository walksRepo, IOwnerRepository ownerRepo, IDogRepository dogRepo)
        {
            _walkerRepo = walkerRepo;
            _walksRepo = walksRepo;
            _ownerRepo = ownerRepo;
            _dogRepo = dogRepo;
        }
        // GET: WalkersController
        public ActionResult Index()
        {
            int currentUserId = GetCurrentUserId();
            return RedirectToAction("Details", new { id = currentUserId });
        }
        

        // GET: WalkersController/Details/5
        public ActionResult Details(int id)
        {
            int currentUserId = GetCurrentUserId();
            if(currentUserId != id)
            {
                return NotFound();
            }
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walks> walks = _walksRepo.GetWalksByWalkerId(walker.Id);
            int totalWalkSeconds = walks.Sum(w => w.Duration);
            TimeSpan walkTime = TimeSpan.FromSeconds(totalWalkSeconds);
            string walkTimeDisplay = $"{walkTime.Hours}Hr {walkTime.Minutes}min";


            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                Walker = walker,
                Walks = walks,
                TotalTimeWalkedDisplay = walkTimeDisplay
             
            };
            if (walker == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Walker walker)
        {
            try
            {
                _walkerRepo.AddWalker(walker);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(walker);
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            Walker walker = _walkerRepo.GetWalkerByEmail(viewModel.Email);

            if (walker == null)
            {
                return Unauthorized();
            }

            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, walker.Id.ToString()),
        new Claim(ClaimTypes.Email, walker.Email),
        new Claim(ClaimTypes.Role, "DogWalker"),
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Walkers");
        }
    }
}
