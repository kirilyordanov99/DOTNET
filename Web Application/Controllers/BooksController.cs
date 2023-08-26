using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web_Application.Data;
using Web_Application.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Web_Application.Controllers
{
    [Authorize] // Restrict access to authenticated users
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();

                booksQuery = booksQuery.Where(book =>
                    EF.Functions.Like(book.ISBN, $"%{searchQuery}%") ||
                    EF.Functions.Like(book.Title, $"%{searchQuery}%") ||
                    EF.Functions.Like(book.Author, $"%{searchQuery}%") ||
                    EF.Functions.Like(book.Genre, $"%{searchQuery}%"));
            }

            var totalItems = booksQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var books = booksQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var model = new BooksIndexViewModel
            {
                Books = books,
                SearchQuery = searchQuery,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return View(model);
        }





        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ISBN,Title,Author,Genre,Price,DateOfPublishing")] Books book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,ISBN,Title,Author,Genre,Price,DateOfPublishing")] Books book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
        [HttpGet]
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "The book has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}