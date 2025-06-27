namespace BibliotecaAPI.DTO
{
    public class AutorConLibrosDTO: AutorCreacionConFotoDTO //hereda de autorDTO tiene id y nombre completo
    {
        public List<LibroDTO> Libros { get; set; } = []; //new
    }
}
