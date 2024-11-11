using Bookfinder.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SeuProjeto.Services
{
    public class OpenLibraryService
    {
        private readonly HttpClient _httpClient;

        public OpenLibraryService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            // URL da API
            var url = "https://openlibrary.org/subjects/love.json?limit=10";
            var response = await _httpClient.GetStringAsync(url);
            dynamic result = JsonConvert.DeserializeObject(response);

            var books = new List<Book>();

            foreach (var item in result.works)
            {
                books.Add(new Book
                {
                    Title = item.title,
                    Author = item.authors[0].name,
                    Key = item.key,
                    Cover = item.cover_id == null ? null : $"https://covers.openlibrary.org/b/id/{item.cover_id}-L.jpg" // Modifica para pegar a URL da imagem
        });
            }

            return books;
        }
    }
}