namespace BibliotecaAPI.DTO
{
    public class AutorCreacionConFotoDTO: AutorCreacionDTO
    {
        //hereda de autorcreacion y le agregamos la propiedad para recibir la foto
        public IFormFile? Foto { get; set; } //representa archivos
    }
}
