using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using MoviesAdmin.Data; // Removed: not needed, context is in global namespace
using MoviesAdmin.Models;

namespace MoviesAdmin.Controllers
{
    public class AudienceRatingsController : Controller
    {
        private readonly MoviesAdminContext _context;

        public AudienceRatingsController(MoviesAdminContext context)
        {
            _context = context;
        }

        // GET: AudienceRatings
        public async Task<IActionResult> Index()
        {
            return View(await _context.AudienceRating.ToListAsync());
        }

        // GET: AudienceRatings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var audienceRating = await _context.AudienceRating
                .FirstOrDefaultAsync(m => m.Id == id);
            if (audienceRating == null) return NotFound();
            return View(audienceRating);
        }

        // GET: AudienceRatings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AudienceRatings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] AudienceRating audienceRating)
        {
            if (ModelState.IsValid)
            {
                _context.Add(audienceRating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(audienceRating);
        }

        // GET: AudienceRatings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var audienceRating = await _context.AudienceRating.FindAsync(id);
            if (audienceRating == null) return NotFound();
            return View(audienceRating);
        }

        // POST: AudienceRatings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] AudienceRating audienceRating)
        {
            if (id != audienceRating.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(audienceRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AudienceRatingExists(audienceRating.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(audienceRating);
        }

        // GET: AudienceRatings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var audienceRating = await _context.AudienceRating
                .FirstOrDefaultAsync(m => m.Id == id);
            if (audienceRating == null) return NotFound();
            return View(audienceRating);
        }

        // POST: AudienceRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var audienceRating = await _context.AudienceRating.FindAsync(id);
            if (audienceRating != null)
            {
                _context.AudienceRating.Remove(audienceRating);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AudienceRatingExists(int id)
        {
            return _context.AudienceRating.Any(e => e.Id == id);
        }
    }
}
