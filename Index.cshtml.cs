using Azure;
using BookManagementStore.Models;
using BookStore.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<BookLibrary> FavouriteBooks { get; set; }

        public List<BookLibrary> Bkstore { get; set; }

        public List<Categories> Categories { get; set; }

      

  
        private HttpClient _client =  new HttpClient();

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient("BOOKSTOREAPI");
        }
        public void OnGet()
        {
            HttpResponseMessage response2 = _client.GetAsync("FavouriteBooks").Result;
            if (response2.IsSuccessStatusCode)
            {
                string responseData2 = response2.Content.ReadAsStringAsync().Result;
                var fb = Newtonsoft.Json.JsonConvert.DeserializeObject<FavouriteBooks>(responseData2);

                if (fb != null && fb.FvrtBooks != null)
                {
                    FavouriteBooks = fb.FvrtBooks;
                }
                else
                {
                    FavouriteBooks = new List<BookLibrary>();
                    TempData["ErrorMessage"] = "No favourite books found.";
                }
            }
            else
            {
                FavouriteBooks = new List<BookLibrary>();
                TempData["ErrorMessage"] = "Failed to load FavouriteBooks data from the API. Please try again later.";
            }


            HttpResponseMessage response = _client.GetAsync("IndexData").Result;

            if (response.IsSuccessStatusCode)
            {
                string responseData = response.Content.ReadAsStringAsync().Result;
                var indexData = Newtonsoft.Json.JsonConvert.DeserializeObject<IndexData>(responseData);

                if (indexData != null)
                {
                    Bkstore = indexData.FeaturedBooks ?? new List<BookLibrary>();

                }

              
            }
            else
            {
                Bkstore = new List<BookLibrary>();
                Categories = new List<Categories>();
                TempData["ErrorMessage"] = "Failed to load Data from the API. Please try again later.";
            }
        }



    }
}
