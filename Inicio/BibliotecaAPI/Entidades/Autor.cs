using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace BibliotecaAPI.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(150,ErrorMessage ="El campo{0} debe tener {1} caracteres o menos")]
        
        public required string Nombres { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]

        public required string Apellidos { get; set; }
        [StringLength(20, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]
        public string? Identificacion { get; set; }
        public List<AutorLibro> Libros { get; set; } = [];

        [Unicode(false)] //crearia la columna varchar con menos caracteres 
        public string? Foto { get; set; }
    }
}
