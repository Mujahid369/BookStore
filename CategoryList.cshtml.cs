using Azure.Core;
using BookManagementStore.Models;
using BookStore.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BookManagementStore.Pages
{
    public class CategoryListModel : PageModel
    {
        private readonly ILogger<CategoryListModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly BookDbContext _bookDbContext;

       
        public CategoryListModel(ILogger<CategoryListModel> logger, BookDbContext context)
        {
            _logger = logger;
            _bookDbContext = context;
        }

        [BindProperty]
        public Categories Category { get; set; }
        public List<Categories> cg { get; set; }
        public void OnGet()
        {
            cg = _bookDbContext.categories.ToList();
        }

        public async Task<IActionResult> OnPost()
        {

            var userName = User.Identity.Name;

           

            var category = new Categories()
            {
                CategoryName = Category.CategoryName,
                Date = DateTime.Now,
                AddedBy = userName,
                Popularity = Category.Popularity,
            };

            _bookDbContext.categories.Add(category);
            _bookDbContext.SaveChanges();
            return new JsonResult("Added Successfully");
        }
        public async Task<IActionResult> OnPostEditCategory(Guid d)
        {
            var categoryToUpdate = cg.FirstOrDefault(c => c.CategoryId == Category.CategoryId);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.CategoryName = Category.CategoryName;
            }
                return new JsonResult("Data received successfully");

        }
        public async Task<IActionResult> OnPostDeleteCategory(Guid id)
        {
            var ctg = _bookDbContext.categories.Find(id);
           
            if (ctg != null)
            {
                _bookDbContext.categories.Remove(ctg);
                _bookDbContext.SaveChanges();
            }
          

            TempData["SuccessMessage"] = "Book Deleted Successfully";
            return new JsonResult("Success");

        }
    }
}
