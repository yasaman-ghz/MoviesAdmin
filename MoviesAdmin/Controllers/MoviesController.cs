using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using MoviesAdmin.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;
    private readonly IAzuriteStorageService _azuriteStorageService;

    public MoviesController(MoviesAdminContext context, IAzuriteStorageService azuriteStorageService)
    {
        _context = context;
        _azuriteStorageService = azuriteStorageService;
    }

    // GET: MOVIES
    public async Task<IActionResult> Index()
    {
        return View(await _context.Movie
            .Include(m => m.Genre)
            .Include(m => m.AudienceRating)
            .AsSplitQuery()
            .OrderByDescending(m => m.ReleaseDate)
            .ToListAsync());
    }

    // GET: MOVIES/Search 
    public async Task<IActionResult> Search(string searchString)
    {
        var movies = _context.Movie.AsQueryable();
        if (!string.IsNullOrEmpty(searchString))
        {
            movies = movies.Where(m => m.Title.Contains(searchString));
            return View(await movies
                .Include(m => m.Genre)
                .Include(m => m.AudienceRating)
                .AsSplitQuery()
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync());
        }

        return View(await _context.Movie.OrderByDescending(m => m.ReleaseDate).ToListAsync());
    }

    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .Include(m => m.Genre)
            .Include(m => m.AudienceRating)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        ViewBag.ReviewsCount = await _context.CriticReview
            .CountAsync(r => r.MovieId == id && r.IsPublished);

        return View(movie);
    }

    // GET: MOVIES/Create
    [Authorize(Roles = "administrator")]
    public IActionResult Create()
    {
        ViewBag.GenreId = new SelectList(_context.Genre, "Id", "Title");
        ViewBag.AudienceRatingId = new SelectList(_context.AudienceRating, "Id", "Title");
        return View();
    }

    // POST: MOVIES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Create([Bind("Id,Title,GenreId,Synopsis,AudienceRatingId,Runtime,ReleaseDate")] Movie movie, IFormFile coverPhoto)
    {
        if (coverPhoto == null)
        {
            ModelState.AddModelError("coverPhoto", "Image file is required.");
        }

        if (ModelState.IsValid)
        {
            string extension = Path.GetExtension(coverPhoto!.FileName);
            string newFileName = $"{Guid.NewGuid()}{extension}";

            string url = await _azuriteStorageService.UploadFileAsync(coverPhoto, newFileName);
            movie.ImageUrl = url;

            _context.Add(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.GenreId = new SelectList(_context.Genre, "Id", "Title", movie.GenreId);
        ViewBag.AudienceRatingId = new SelectList(_context.AudienceRating, "Id", "Title", movie.AudienceRatingId);
        return View(movie);
    }

    // GET: MOVIES/Edit/5
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        ViewBag.GenreId = new SelectList(_context.Genre, "Id", "Title", movie.GenreId);
        ViewBag.AudienceRatingId = new SelectList(_context.AudienceRating, "Id", "Title", movie.AudienceRatingId);
        return View(movie);
    }

    // POST: MOVIES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,GenreId,Synopsis,AudienceRatingId,ImageUrl,Runtime,ReleaseDate")] Movie movie, IFormFile? coverPhoto)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (coverPhoto != null)
                {
                
                    if (!string.IsNullOrEmpty(movie.ImageUrl))
                    {
                        await _azuriteStorageService.DeleteFileAsync(movie.ImageUrl);
                    }

                    string extension = Path.GetExtension(coverPhoto.FileName);
                    string newFileName = $"{Guid.NewGuid()}{extension}";
                    string url = await _azuriteStorageService.UploadFileAsync(coverPhoto, newFileName);
                    movie.ImageUrl = url;
                }

                _context.Update(movie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.Id))
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

        ViewBag.GenreId = new SelectList(_context.Genre, "Id", "Title", movie.GenreId);
        ViewBag.AudienceRatingId = new SelectList(_context.AudienceRating, "Id", "Title", movie.AudienceRatingId);
        return View(movie);
    }

    // GET: MOVIES/Delete/5
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .Include(m => m.Genre)
            .Include(m => m.AudienceRating)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: MOVIES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie != null)
        {
         
            if (!string.IsNullOrEmpty(movie.ImageUrl))
            {
                await _azuriteStorageService.DeleteFileAsync(movie.ImageUrl);
            }
            _context.Movie.Remove(movie);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MovieExists(int? id)
    {
        return _context.Movie.Any(e => e.Id == id);
    }
}
