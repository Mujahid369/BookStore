using Azure.Core;
using BookManagementStore.FileUploadService;
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

namespace BookManagementStore.Pages
{
    
    public class ListModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IFileUploadService _fileUploadService;
        private readonly string apiUrl = "https://localhost:7099/api/books";
        private readonly IWebHostEnvironment _environment;
        private readonly BookDbContext _bookDbContext;
        [BindProperty]
        public List<BookLibrary> bk { get; set; }
        public BookLibrary sBookLibrary { get; set; }
        public DbSet<BookLibraryImage> BookImg { get; set; }
        public List<Categories> categories { get; set; }

        public ListModel(IWebHostEnvironment environment, BookDbContext bookDbContext, IHttpClientFactory httpClientFactory)
        {
            _environment = environment;
            _bookDbContext = bookDbContext;
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult OnPostSuccess()
        {
            // Show success message (optional)
            TempData["SuccessMessage"] = "Updated Successfully.";

            // Redirect to another page
            return RedirectToPage("/List"); // Replace "PageList" with the name of your page
        }
        public IActionResult OnPostError()
        {
            // Pass error message to the client-side JavaScript
            TempData["ErrorMessage"] = "An error occurred while processing your request.";

            // Redirect to another page or stay on the same page, based on your requirement
            return RedirectToPage("/ErrorPage"); // Replace "ErrorPage" with the name of your error page
        }

        public void OnGet()
        
        {
            categories = _bookDbContext.categories.ToList();

            bk = _bookDbContext.Booklibrarys
             .Include(x => x.BookImages)
             .Include(x => x.Categories)  
               .ToList();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };
        }

      
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            try
            {
               
                var response = await _httpClient.DeleteAsync($"{apiUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book Deleted Successfully";
                    return new JsonResult("Success");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting the book.";
                    return new JsonResult("Error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the book.";
                return new JsonResult("Error");
            }
        }


        public async Task<IActionResult> OnPostDeleteImage(Guid id)
        {
            // Ensure _environment is not null
            if (_environment == null)
            {
                TempData["ErrorMessage"] = "IWebHostEnvironment is not initialized.";
                return RedirectToPage("/List");
            }

            var image = _bookDbContext.BookImg.Find(id);
            if (image != null)
            {
                // Check if ImagePath is not null or empty
                if (string.IsNullOrEmpty(image.ImagePath))
                {
                    TempData["ErrorMessage"] = "Image path is null or empty.";
                    return RedirectToPage("/List");
                }

                // Combine the path correctly
                var fullPath = Path.Combine(_environment.ContentRootPath, image.ImagePath);

                // Log the full path for debugging purposes
                Console.WriteLine("Full path: " + fullPath);

                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Error deleting image from file system: " + ex.Message;
                        return RedirectToPage("/List");
                    }
                }

                _bookDbContext.BookImg.Remove(image);
                _bookDbContext.SaveChanges();
                TempData["SuccessMessage"] = "Image deleted successfully.";
                return new JsonResult("Success");
            }

            TempData["ErrorMessage"] = "Image not found.";
            return RedirectToPage("/List");
        }

    }
}
