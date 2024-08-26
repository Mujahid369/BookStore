using BookManagementStore.Models;
using BookStore.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BookManagementStore.Pages
{
    public class addcartModel : PageModel
    {
        private readonly BookDbContext _context;
        [BindProperty]
        public BookLibrary sBookLibrary { get; set; }
        public List<BookLibraryImage> Images { get; set; }
        public addcartModel(BookDbContext bookDbContext)
        {
            this._context = bookDbContext;

        }

        public void OnGet(Guid id)
        {
            sBookLibrary = _context.Booklibrarys
                    .Include(x => x.BookImages)
                    .FirstOrDefault(x => x.BookId == id);

            //Images = _context.BookImg.ToList();



            //foreach (var item in Images)
            //{
            //_context.BookImg.Include(a => a.ImagePath);
            //}

        }

    }
}
