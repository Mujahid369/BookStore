using BookManagementStore.Models;
using BookStore.data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BookManagementStore.Pages
{
    public class BooksModel : PageModel
    {
        private readonly BookDbContext _dbContext;

        public List<BookLibrary> Books { get; set; }
        public List<Categories> Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCategoriesString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedISBN { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PriceRange { get; set; }

        public BooksModel(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {
            IQueryable<BookLibrary> booksQuery = _dbContext.Booklibrarys;

            if (!string.IsNullOrEmpty(SelectedCategoriesString))
            {
                var selectedCategoryIds = SelectedCategoriesString.Split(',')
                    .Select(id => Guid.Parse(id))
                    .ToList();

                booksQuery = booksQuery.Where(b => selectedCategoryIds.Contains(b.CategoryId));
            }

            if (!string.IsNullOrEmpty(SelectedISBN) && int.TryParse(SelectedISBN, out int selectedISBN))
            {
                booksQuery = booksQuery.Where(b => b.ISBAN == selectedISBN);
            }

            var books = booksQuery.ToList();

            if (!string.IsNullOrEmpty(PriceRange) && decimal.TryParse(PriceRange, out decimal maxPrice))
            {
                books = books.Where(b =>
                    !string.IsNullOrEmpty(b.SoldPrice) &&
                    decimal.TryParse(b.SoldPrice, out decimal soldPrice) &&
                    soldPrice <= maxPrice
                ).ToList();
            }

            Books = books;
            Categories = _dbContext.categories.ToList();
        }
    }
}
