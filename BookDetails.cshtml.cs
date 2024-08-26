
using BookManagementStore.Models;
using BookStore.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManagementStore.Pages
{
    public class BookDetailsModel : PageModel
    { 
     private readonly BookDbContext _context;
    [BindProperty]
    public BookLibrary BookLibrary { get; set; }
      public List <BookLibraryImage> Images { get; set; }
    public BookDetailsModel(BookDbContext bookDbContext)
    {
        this._context = bookDbContext;

    }



        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            BookLibrary = await _context.Booklibrarys
                .Include(x => x.BookImages)
                .FirstOrDefaultAsync(x => x.BookId == id);

            if (BookLibrary == null)
            {
                return NotFound();
            }

            return Partial("_Details", BookLibrary);
        }


        public void OnPost()
    {
        //    Images = _context.BookImg.ToList();
        //    foreach (var item in Images)
        //    {
        //        var img = _context.BookImg.Include(a => a.ImagePath);
        //    }
        //    var Bks = _context.Booklibrarys.Find(sBookLibrary.BookId);
        //if (Bks != null)
        //{
        //    Bks.Author = sBookLibrary.Author;
        //    Bks.Image = sBookLibrary.Image;
        //    Bks.BookName = sBookLibrary.BookName;
        //    Bks.ISBAN = sBookLibrary.ISBAN;
        //    Bks.Description = sBookLibrary.Description;
            
        //}

    }
}
}
