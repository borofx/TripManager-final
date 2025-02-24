using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TripManager.Data;
using TripManager.Models;
using Microsoft.EntityFrameworkCore;

namespace TripManager.Controllers
{
    [Authorize]
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TourController> _logger;

        public TourController(ApplicationDbContext context, UserManager<User> userManager, ILogger<TourController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManageLandmarks()
        {
            var landmarks = _context.Landmarks.ToList();
            return View(landmarks);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteLandmark(int id)
        {
            var landmark = await _context.Landmarks.FindAsync(id);
            if (landmark != null)
            {
                _context.Landmarks.Remove(landmark);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageLandmarks));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditLandmark(int id)
        {
            var landmark = await _context.Landmarks.FindAsync(id);
            if (landmark == null) return NotFound();
            return View(landmark);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditLandmark(Landmark landmark)
        {
            if (ModelState.IsValid)
            {
                _context.Landmarks.Update(landmark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageLandmarks));
            }
            return View(landmark);
        }

        public async Task<IActionResult> MyTours()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tours = await _context.Tours
                .Where(t => t.UserId == userId)
                .Include(t => t.TourLandmarks)
                    .ThenInclude(tl => tl.Landmark)
                .ToListAsync();

            return View(tours);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var userId = _userManager.GetUserId(User);
            var tour = await _context.Tours
                .Include(t => t.TourLandmarks)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (tour != null)
            {
                _context.TourLandmarks.RemoveRange(tour.TourLandmarks);

                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyTours));
        }

        [HttpGet]
        public async Task<IActionResult> CreateTour()
        {
            var landmarks = await _context.Landmarks.ToListAsync();
            ViewBag.Landmarks = landmarks;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour(Tour tour, int[] selectedLandmarkIds)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                var landmarks = await _context.Landmarks.ToListAsync();
                ViewBag.Landmarks = landmarks;
                return View(tour);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var newTour = new Tour
            {
                Name = tour.Name,
                Description = tour.Description,
                UserId = userId,
                TourLandmarks = new List<TourLandmark>()
            };

            if (selectedLandmarkIds != null && selectedLandmarkIds.Length > 0)
            {
                foreach (var landmarkId in selectedLandmarkIds)
                {
                    var landmark = await _context.Landmarks.FindAsync(landmarkId);
                    if (landmark != null)
                    {
                        newTour.TourLandmarks.Add(new TourLandmark
                        {
                            LandmarkId = landmarkId
                        });
                    }
                }
            }

            _context.Tours.Add(newTour);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyTours));
        }
        public IActionResult Map()
        {
            return View();
        }

        [HttpGet]
        [Route("api/tours")]
        public async Task<IActionResult> GetTours()  // Changed return type to IActionResult
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tours = await _context.Tours
                .Where(t => t.UserId == userId)
                .Include(t => t.TourLandmarks)
                    .ThenInclude(tl => tl.Landmark)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                    Landmarks = t.TourLandmarks.Select(tl => new
                    {
                        tl.Landmark.Id,
                        tl.Landmark.Name,
                        tl.Landmark.Latitude,
                        tl.Landmark.Longitude                        
                    })
                })
                .ToListAsync();

            return Json(tours);
        }
    }
}
