﻿using Bookfinder.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bookfinder.Service
{
    public class OpenLibraryService:InterfaceAPI
    {
        private readonly HttpClient _httpClient;

        public OpenLibraryService()
        {
            _httpClient = new HttpClient();
        }

        public virtual async Task<List<Book>> GetBooksAsync()
        {
           
            var url = "https://openlibrary.org/subjects/love.json?limit=15";
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

        public async Task<Book> GetBookDetailsAsync(string bookKey)
        {

           
            var url = $"https://openlibrary.org{bookKey}.json";

            var response = await _httpClient.GetStringAsync(url);
            dynamic result = JsonConvert.DeserializeObject(response);

            
            var book = new Book
            {
                Title = result.title,
                Author = result.authors[0].name != null && result.authors.Count > 0 ? result.authors[0].name : "Autor desconhecido", 
                Key = result.key,
                Cover = $"https://covers.openlibrary.org/b/id/{result.covers[0]}-L.jpg" 

            };


            return book;
        }
    }
}