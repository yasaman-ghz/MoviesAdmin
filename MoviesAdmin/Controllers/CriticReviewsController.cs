using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using MoviesAdmin; 

[Authorize(Roles = "critic")]
public class CriticReviewsController : Controller
{
    private readonly MoviesAdminContext _context;
    private string? _userId;

    public CriticReviewsController(MoviesAdminContext context)
    {
        _context = context;
    }

    // Overriding OnActionExecuting to automatically resolve the user identity context before filters execute
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        _userId = User.GetUserId();
    }

    // GET: CriticReviews
    public async Task<IActionResult> Index()
    {
       
        var criticReviews = await _context.CriticReview
            .Include(review => review.Movie)
            .Where(review => review.CreatedBy == _userId)
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
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == _userId);

        if (criticReview == null)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    // GET: CriticReviews/Create?movieId=5
    public async Task<IActionResult> Create(int? movieId)
    {
        if (movieId == null)
        {
            return BadRequest("A valid Movie ID context must be supplied.");
        }

        var movie = await _context.Movie.FindAsync(movieId);
        if (movie == null)
        {
            return NotFound();
        }

        bool alreadyReviewed = await _context.CriticReview
            .AnyAsync(r => r.MovieId == movieId && r.CreatedBy == _userId);

        if (alreadyReviewed)
        {
            TempData["ErrorMessage"] = "You have already submitted a review for this movie.";
            return RedirectToAction("Details", "Movies", new { id = movieId });
        }

        var review = new CriticReview
        {
            MovieId = movieId.Value,
            Movie = movie
        };

        return View(review);
    }

    // POST: CriticReviews/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MovieId,Title,Description,Rating,IsPublished")] CriticReview criticReview)
    {
        criticReview.CreatedBy = _userId;

        //Converts current standard system datetime values into the required DateOnly
        criticReview.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

    
        bool alreadyReviewed = await _context.CriticReview
            .AnyAsync(r => r.MovieId == criticReview.MovieId && r.CreatedBy == _userId);

        if (alreadyReviewed)
        {
            ModelState.AddModelError("", "You can only publish one review per movie.");
        }

        if (ModelState.IsValid)
        {
            _context.CriticReview.Add(criticReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        criticReview.Movie = await _context.Movie.FindAsync(criticReview.MovieId);
        return View(criticReview);
    }

    // GET: CriticReviews/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Include(r => r.Movie)
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == _userId);

        if (criticReview == null)
        {
            return NotFound();
        }

      
        if (criticReview.IsPublished)
        {
            return Forbid();
        }

        return View(criticReview);
    }

    // POST: CriticReviews/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,Title,Description,Rating,IsPublished,CreatedBy,CreatedDate")] CriticReview criticReview)
    {
        if (id != criticReview.Id)
        {
            return NotFound();
        }

        var existingReview = await _context.CriticReview
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == _userId);

        if (existingReview == null)
        {
            return NotFound();
        }

   
        if (existingReview.IsPublished)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(criticReview);
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

        criticReview.Movie = await _context.Movie.FindAsync(criticReview.MovieId);
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
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == _userId);

        if (criticReview == null)
        {
            return NotFound();
        }

        if (criticReview.IsPublished)
        {
            return Forbid();
        }

        return View(criticReview);
    }

    // POST: CriticReviews/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var criticReview = await _context.CriticReview
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == _userId);

        if (criticReview == null)
        {
            return NotFound();
        }

        if (criticReview.IsPublished)
        {
            return Forbid();
        }

        _context.CriticReview.Remove(criticReview);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CriticReviewExists(int id)
    {
        return _context.CriticReview.Any(e => e.Id == id);
    }
}

