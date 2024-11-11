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

            // Verifica se o livro já existe no banco de dados
            var existingBook = await _context.Books
                .SingleOrDefaultAsync(b => b.Key == bookKey);

            if (existingBook == null)
            {
                // Se o livro não existir, pegue seus detalhes da API
                var bookDetails = await _service.GetBookDetailsAsync(bookKey);

                // Crie um novo livro com os detalhes
                var book = new Book
                {
                    Key = bookKey,
                    Title = bookDetails.Title,
                    Author = bookDetails.Author,
                    Cover = bookDetails.Cover,
                    IsFavorited = true // Define que o livro está favoritado
                };

                // Adiciona o livro ao contexto
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Livro favoritado com sucesso!";
                ViewBag.Style = "alert alert-success";
            }
            else
            {
                ViewBag.Message = "Este livro já está na sua lista de favoritos.";
                ViewBag.Style = "alert alert-danger";
            }

            // Obtém a lista de livros da API para mostrar novamente na view
            var books = await _service.GetBooksAsync();
            return View("Index", books);
        }

        public async Task<IActionResult> FavoriteBooks()
        {
            // Obtém todos os livros que estão no banco de dados
            var favoriteBooks = await _context.Books
                .Where(b => b.IsFavorited) // Filtra livros favoritados
                .ToListAsync();

            return View(favoriteBooks); // Retorna a view com a lista de livros favoritos
        }

        public async Task<IActionResult> DeleteFavorite(string bookKey)
        {
            if (string.IsNullOrEmpty(bookKey))
            {
                return BadRequest(); // Trata caso de chave vazia
            }

            // Procura o livro na tabela
            var book = await _context.Books
                .SingleOrDefaultAsync(b => b.Key == bookKey);

            if (book != null)
            {
                // Remove o livro do contexto
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Livro removido dos favoritos com sucesso!";
            }
            else
            {
                TempData["Message"] = "Livro não encontrado nos favoritos.";
            }

            // Redireciona de volta para a lista de livros favoritos
            return RedirectToAction("FavoriteBooks");
        }

        // GET: Formulário para adicionar resenha
        public IActionResult AddReview(int bookId)
        {
            ViewBag.BookId = bookId;

            // Carrega as resenhas associadas ao livro
            ViewBag.Reviews = _context.Reviews
                .Where(r => r.BookId == bookId) // Filtra resenhas pelo ID do livro
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Review review)
        {
            Console.WriteLine($"BookId recebido: {review.BookId}"); // Verifica qual BookId está sendo recebido

            // Verifica se o livro existe
            var bookExists = await _context.Books.AnyAsync(b => b.Id == review.BookId);
            if (!bookExists)
            {
                ModelState.AddModelError("", "O livro associado à resenha não existe.");
                ViewBag.BookId = review.BookId; // Retorna o BookId para a view
                ViewBag.Reviews = await _context.Reviews
                    .Where(r => r.BookId == review.BookId)
                    .ToListAsync();
                return View(review); // Retorna a view com erro
            }

            review.CreatedAt = DateTime.Now; // Define a data de criação
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Resenha adicionada com sucesso!";
            return RedirectToAction("FavoriteBooks"); // Redireciona para a lista de livros favoritos
        }
    }


}

