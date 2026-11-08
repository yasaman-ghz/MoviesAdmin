using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

namespace MoviesAdmin.Controllers
{
    public class CriticReviewsController : Controller
    {
        private readonly MoviesAdminContext _context;

        public CriticReviewsController(MoviesAdminContext context)
        {
            _context = context;
        }

        // GET: CriticReviews
        public async Task<IActionResult> Index()
        {
            var criticReviews = await _context.CriticReview
                .Include(review => review.Movie)
                .AsNoTracking()
                .ToListAsync();

            return View(criticReviews);
        }

        // GET: CriticReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var criticReview = await _context.CriticReview
                .Include(review => review.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (criticReview == null)
            {
                return NotFound();
            }

            return View(criticReview);
        }

        // GET: CriticReviews/Create
        public IActionResult Create()
        {
            PopulateMovies();
            return View();
        }

        // POST: CriticReviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Description,Rating,IsPublished,CreatedBy,CreatedDate")]
            CriticReview criticReview)
        {
            if (ModelState.IsValid)
            {
                _context.CriticReview.Add(criticReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateMovies(criticReview.MovieId);
            return View(criticReview);
        }

        // GET: CriticReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var criticReview = await _context.CriticReview.FindAsync(id);
            if (criticReview == null)
            {
                return NotFound();
            }

            PopulateMovies(criticReview.MovieId);
            return View(criticReview);
        }

        // POST: CriticReviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,Title,Description,Rating,IsPublished,CreatedBy,CreatedDate")]
            CriticReview criticReview)
        {
            if (id != criticReview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.CriticReview.Update(criticReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CriticReviewExists(criticReview.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateMovies(criticReview.MovieId);
            return View(criticReview);
        }

        // GET: CriticReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var criticReview = await _context.CriticReview
                .Include(review => review.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (criticReview == null)
            {
                return NotFound();
            }

            return View(criticReview);
        }

        // POST: CriticReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var criticReview = await _context.CriticReview.FindAsync(id);
            if (criticReview != null)
            {
                _context.CriticReview.Remove(criticReview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CriticReviewExists(int id)
        {
            return _context.CriticReview.Any(e => e.Id == id);
        }

        private void PopulateMovies(int? selectedMovieId = null)
        {
            ViewData["MovieId"] = new SelectList(
                _context.Movie.AsNoTracking().OrderBy(movie => movie.Title),
                "Id",
                "Title",
                selectedMovieId);
        }
    }
}