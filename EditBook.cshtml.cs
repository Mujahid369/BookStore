using BookManagementStore.Models;
using BookManagementStore.FileUploadService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookManagementStore.Pages
{
    public class EditBookModel : PageModel

    {
        private readonly HttpClient _httpClient;
        private readonly IFileUploadService _fileUploadService;
        private readonly string apiUrl = "https://localhost:7099/api/books";

        [BindProperty]
        public BookLibrary BkLib { get; set; }
        public List<string> FileNames { get; set; }
        public List<BookLibraryImage> BookImages { get; set; }

        public EditBookModel(IFileUploadService fileUploadService, IHttpClientFactory httpClientFactory)
        {
            _fileUploadService = fileUploadService;
            
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                BkLib = await response.Content.ReadFromJsonAsync<BookLibrary>();
                if (BkLib != null)
                {
                    var imagesResponse = await _httpClient.GetAsync($"{apiUrl}/{id}/images");
                    if (imagesResponse.IsSuccessStatusCode)
                    {
                        BookImages = await imagesResponse.Content.ReadFromJsonAsync<List<BookLibraryImage>>();
                    }
                }
                return Page();
            }
            else
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToPage("/List");
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid[] selectedImages, IFormFile[] BookImages, IFormFile Image)
        {
            try
            {
                var bookUpdate = new BookLibrary
                {
                    BookId = BkLib.BookId,
                    Author = BkLib.Author,
                    BookName = BkLib.BookName,
                    ISBAN = BkLib.ISBAN,
                    Description = BkLib.Description,
                    Favourites = BkLib.Favourites,
                    Image = Image != null
                        ? await _fileUploadService.UploadFileAsync(Image)
                        : BkLib.Image,
                       

                };
                var fileNames = await _fileUploadService.UploadMultiFileAsync(BookImages);
                var images = fileNames.Select(fileName => new BookLibraryImage
                {
                    BookId = BkLib.BookId,
                    ImagePath = fileName
                }).ToList();

                var updateJsonContent = new StringContent(JsonSerializer.Serialize(bookUpdate), Encoding.UTF8, "application/json");
                var updateResponse = await _httpClient.PutAsync($"{apiUrl}/{BkLib.BookId}", updateJsonContent);

                if (!updateResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error updating the book.";
                    return RedirectToPage("/List");
                }

                
               
                

                if (selectedImages != null && selectedImages.Any())
                {
                    var deleteJsonContent = new StringContent(JsonSerializer.Serialize(selectedImages), Encoding.UTF8, "application/json");
                    var deleteResponse = await _httpClient.PostAsync($"{apiUrl}/{BkLib.BookId}/deleteimages", deleteJsonContent);

                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Error deleting selected images.";
                        return RedirectToPage("/List");
                    }
                }

                TempData["SuccessMessage"] = "Book updated successfully.";
                return RedirectToPage("/List");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while updating the book.";
                return RedirectToPage("/List");
            }
        }
    }
}