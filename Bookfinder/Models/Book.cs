using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookfinder.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
   
        public String Title { get; set; }

        public String Author { get; set; }

        public string Key { get; set; }

        public string? Cover { get ; set; }

        public bool IsFavorited { get; set; } // Adicionando a propriedade favoritado


    }
}
