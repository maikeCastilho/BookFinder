using System.ComponentModel.DataAnnotations;

namespace Bookfinder.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; } // Conteúdo da resenha

        public int FavoriteBookId { get; set; } // ID do livro favorito associado
        public virtual FavoriteBook FavoriteBook { get; set; } // Relacionamento com FavoriteBook
    }
}
