using System.ComponentModel.DataAnnotations;

namespace Bookfinder.Models
{
    public class FavoriteBook
    {
        [Key]
        public int Id { get; set; }
        public string BookKey { get; set; } // A chave única do livro da API
        public int UserId { get; set; }

        // Novas propriedades para armazenar informações do livro
        public string Title { get; set; }
        public string Author { get; set; }
        public string Cover { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } // Relacionamento com as Resenhas
    }
}
