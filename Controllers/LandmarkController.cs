using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripManager.Data;
using TripManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TripManager.Controllers
{
    [Authorize]
    public class LandmarkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LandmarkController(ApplicationDbContext context)
        {
            _context = context;
        }

        // API Endpoint to fetch all landmarks as JSON
        [HttpGet]
        [AllowAnonymous] // Anyone can view landmarks
        [Route("api/landmarks")]
        public async Task<IActionResult> GetLandmarks()
        {
            var landmarks = await _context.Landmarks.ToListAsync();
            return Ok(landmarks);  // Returns JSON
        }

        // View List of Landmarks
        public async Task<IActionResult> Index()
        {
            var landmarks = await _context.Landmarks.ToListAsync();
            return View(landmarks);
        }

        // GET: Create a new landmark
        public IActionResult Create()
        {
            return View();
        }

        // POST: Save the new landmark to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Latitude,Longitude")] Landmark landmark)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(landmark);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Landmark created successfully!";
                    return RedirectToAction("ManageLandmarks", "Tour");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating landmark: " + ex.Message);
                }
            }
            return View(landmark);
        }
        public IActionResult Map()
        {
            return View();
        }
    }
}
