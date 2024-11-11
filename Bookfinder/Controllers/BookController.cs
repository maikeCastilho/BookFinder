using Bookfinder.Data;
using Bookfinder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeuProjeto.Services;


namespace Bookfinder.Controllers
{
    public class BookController : Controller
    {
        private readonly MyContext _context;
        private readonly OpenLibraryService _service;


        public BookController(MyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _service = new OpenLibraryService();
        }

      
        public async Task<IActionResult> Index()
        {
            var books = await _service.GetBooksAsync();
            return View(books);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Books.SingleOrDefaultAsync(i => i.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }



        public async Task<IActionResult> Favorite(string bookKey)
        {
            if (string.IsNullOrEmpty(bookKey))
            {
                return BadRequest(); // Trata caso de chave vazia
            }

            // Verifica se o livro já está na tabela FavoriteBooks
            var existingFavorite = await _context.FavoriteBooks
                .SingleOrDefaultAsync(f => f.BookKey == bookKey);

            if (existingFavorite == null)
            {
                // Se o livro não existe como favorito, pegue seus detalhes
                var bookDetails = await _service.GetBookDetailsAsync(bookKey);

                // Crie um novo favorito com os detalhes do livro
                var favoriteBook = new FavoriteBook
                {
                    BookKey = bookKey,
                    UserId = 1, // Defina este ID corretamente se houver controle de usuários
                    Title = bookDetails.Title,
                    Author = bookDetails.Author,
                    Cover = bookDetails.Cover
                };

                _context.FavoriteBooks.Add(favoriteBook);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Livro favoritado com sucesso!";
            }
            else
            {
                ViewBag.Message = "Este livro já está na sua lista de favoritos.";
            }

            // Obtém a lista de livros da API para mostrar novamente na view
            var books = await _service.GetBooksAsync();
            return View("Index", books);
        }


        public async Task<IActionResult> FavoriteBooks()
        {
            var favoriteBooks = await _context.FavoriteBooks
                .ToListAsync(); // Adicione Include se precisar de detalhes do livro

            return View(favoriteBooks); // Retorna a view com a lista de livros favoritos
        }


    }
}
