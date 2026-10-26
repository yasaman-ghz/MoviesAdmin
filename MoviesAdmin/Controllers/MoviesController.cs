using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using MoviesAdmin.Services;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;
    private readonly AzuriteStorageService _azuriteStorageService;

    public MoviesController(MoviesAdminContext context, AzuriteStorageService azuriteStorageService)
    {
        _context = context;
        _azuriteStorageService = azuriteStorageService;
    }

    // GET: MOVIES
    public async Task<IActionResult> Index()
    {     //Listing all movies with the newest shown on top of the list
          //Reference: https://www.learnentityframeworkcore.com/querying/linq-queries
        return View(await _context.Movie.OrderByDescending(Movie=> Movie.ReleaseDate).ToListAsync());
    }

    // New controller method to search for movies by title
    // GET: MOVIES/Search 
    public async Task<IActionResult> Search(string searchString)//passing string from the form                 
                                                                                                     
    {
        //Search bar reference: https://www.youtube.com/watch?v=5-fVog2hd5c
        var movies = from m in _context.Movie
                     select m;
        if (!string.IsNullOrEmpty(searchString))
        {
            movies = movies.Where(m => m.Title.Contains(searchString));
            return View(await movies.OrderByDescending(Movie => Movie.ReleaseDate).ToListAsync());
        }

        return View(await _context.Movie.OrderByDescending(Movie => Movie.ReleaseDate).ToListAsync());
    }

    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // GET: MOVIES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MOVIES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Genre,Synopsis,Rating,Runtime,ReleaseDate")] Movie movie ,IFormFile coverPhoto)
    {
        // 1. Check if the file is missing
        if (coverPhoto == null)
        {
            // The key MUST match the input's name attribute exactly ("coverPhoto")
            ModelState.AddModelError("coverPhoto", "Image file is required.");
        }

        if (ModelState.IsValid) // validate input
        {
            //
            // Upload file to Azure Stroage (after input validation occurs)
            //

            // Make a unique filename (GUID.ext)
            string extension = Path.GetExtension(coverPhoto!.FileName); // includes the leading dot, e.g. ".jpg"
            string newFileName = $"{Guid.NewGuid()}{extension}";

            // Upload file to Azure Storage
            string url = await _azuriteStorageService.UploadFileAsync(coverPhoto, newFileName);

            // Set the image URL in movie record            
            movie.ImageUrl = url;

            //
            // Save movie in database
            //

            _context.Add(movie); // add new movie to context (db)
            await _context.SaveChangesAsync(); // save context changes in db

            return RedirectToAction(nameof(Index)); // re-direct to Movies/Index page
        }
        return View(movie);

    }

    // GET: MOVIES/Edit/5
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
        return View(movie);
    }

    // POST: MOVIES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Genre,Synopsis,Rating,ImageUrl,Runtime,ReleaseDate")] Movie movie, IFormFile? coverPhoto)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }


        if (coverPhoto != null)
        {
            // Make a unique filename (GUID.ext)
            string extension = Path.GetExtension(coverPhoto!.FileName); // includes the leading dot, e.g. ".jpg"
            string newFileName = $"{Guid.NewGuid()}{extension}";

            // Upload file to Azure Storage
            string url = await _azuriteStorageService.UploadFileAsync(coverPhoto, newFileName);

            // Set the image URL in movie record            
            movie.ImageUrl = url;

        }

        if (ModelState.IsValid)
        {
            try
            {
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
        return View(movie);
    }

    // GET: MOVIES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
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
    public async Task<IActionResult> DeleteConfirmed(int? id)     
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie != null)
        {
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
