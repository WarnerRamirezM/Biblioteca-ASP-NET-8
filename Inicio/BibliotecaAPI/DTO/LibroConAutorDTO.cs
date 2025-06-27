using System.Reflection.Metadata.Ecma335;

namespace BibliotecaAPI.DTO
{
    public class LibroConAutoresDTO: LibroDTO //herencia la id y titulo del libro
    {
        public List<AutorCreacionConFotoDTO> Autores { get; set; } = [];
    }
}
