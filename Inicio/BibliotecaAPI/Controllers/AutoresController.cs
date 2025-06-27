using AutoMapper;
using Azure;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTO;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize]
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context; //campo de clase para poder acceder en toda la clase
        private readonly IMapper mapper; //imyeccion de AutoMapper
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private const string contenedor = "autores";
        public AutoresController(ApplicationDbContext context, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos) //inyectando el applicationDBContext
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        //[HttpGet]
        //[AllowAnonymous] //permite utilizar este endpoint independientemente del authorize 
        ////se necesita saber cuanta cantidad de autores hay
        //public async Task<IEnumerable<AutorDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        //{
        //    var autores = await context.Autores.ToListAsync();
        //    var autoresDTO = autores.Select(autor => 
        //    new AutorDTO { Id = autor.Id, NombreCompleto = $"{autor.Nombres} {autor.Apellidos}"});

        //    return autoresDTO;

        //}

        [HttpGet]
        public async Task<IEnumerable<AutorCreacionConFotoDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable(); //es una representacion de la tabla autores pero en MEMORIA
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);//se pasa las representacion de tabla autores
            var autores = await queryable
                .OrderBy(x => x.Nombres) //se ordena por nombre
                .Paginar(paginacionDTO).ToListAsync(); //lista paginada
            var autoresDTO = mapper.Map<IEnumerable<AutorCreacionConFotoDTO>>(autores);

            return autoresDTO;

        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")] // api/autores/id
        [AllowAnonymous]
        [EndpointSummary("Obtiene autor por id")] //resumen del endpoint en swagger
        [EndpointDescription("Obtiene un autor por su id. incluye sus libros. Si el autor no existe, se retorna 404")]
        [ProducesResponseType<AutorConLibrosDTO>(statusCode: 200)] //devuelve la creacion correcta
        [ProducesResponseType(StatusCodes.Status404NotFound)] //devuelve la otra respuesta no encontrado
        public async Task<ActionResult<AutorConLibrosDTO>> Get([Description("El id del autor")]int id)
        {
            var autor = await context.Autores
                .Include(x => x.Libros)
                .ThenInclude(x => x.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorConLibrosDTO>(autor);
            return autorDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor> (autorCreacionDTO);
            context.Add(autor); //agregamos el autor
            await context.SaveChangesAsync(); //guardamos cambios asyn a nivel de bases de datos
            var autorDTO = mapper.Map<AutorCreacionConFotoDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new {id = autor.Id}, autor); //creado por ruta, id cliente, autor creado
        }
        [HttpPost("con-foto")]
        public async Task<ActionResult> PostConFoto([FromForm]AutorCreacionConFotoDTO autorCreacionDTO)
        {
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            if(autorCreacionDTO.Foto is not null) //si existe la foto
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, autorCreacionDTO.Foto); //le mandamos la interfaz almacenamos y pasamos los parametros
                autor.Foto = url; //asignamos a la base de datos la url
            }
            context.Add(autor); //agregamos el autor
            await context.SaveChangesAsync(); //guardamos cambios asyn a nivel de bases de datos
            var autorDTO = mapper.Map<AutorCreacionConFotoDTO>(autor);
            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autor); //creado por ruta, id cliente, autor creado
        }

        [HttpPut("{id:int}")] // api/autores/id
        public async Task<ActionResult> Put(int id, [FromForm]AutorCreacionConFotoDTO autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id==id); //buscamos cualquier id que exista y sea igual al pasado por parametro
            if(!existeAutor) return NotFound(); //si no existe retornamos
            var autor = mapper.Map<Autor>(autorCreacionDTO); //si existe mapeamos el autor
            autor.Id = id; //asignamos identificacion 
            if(autorCreacionDTO.Foto is not null) //si me mandan la foto la editamos
            {
                var fotoActual = await context
                    .Autores.Where(x => x.Id == id)
                    .Select(x => x.Foto)
                    .FirstAsync(); //buscamos la identificacion de autor y la foto que tiene como la primera encontrada
                var url = await almacenadorArchivos.Editar(fotoActual, contenedor, autorCreacionDTO.Foto); //interfaz y le pasamos el metodo editar con la ruta de la foto actual, el contenedor y la foto por parametro nueva
                autor.Foto = url;
            }

            context.Update(autor); //actualizamos 
            await context.SaveChangesAsync();
            return NoContent(); //204 standart API 
        }
        [HttpPatch("{id:int}")] // Indica que este método responde a solicitudes HTTP PATCH con un parámetro 'id' entero en la URL (por ejemplo, PATCH /autores/3)
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc) // Método asincrónico que recibe el ID del autor y un documento PATCH con los cambios
        {
            if (patchDoc == null) // Si el documento patch es null, es decir, no se recibió nada del cliente...
            {
                return BadRequest(); // ...se retorna un error 400 (Bad Request), ya que no se puede procesar la solicitud.
            }

            var autorDB = await context.Autores.FirstOrDefaultAsync(x => x.Id == id); // Se busca en la base de datos el autor con el ID especificado
            if (autorDB == null)
            { // Si no se encuentra ningún autor con ese ID...
                return NotFound(); // ...se retorna un error 404 (Not Found)
            }

            var autorPatchDTO = mapper.Map<AutorPatchDTO>(autorDB); // Se mapea el autor de la base de datos a un DTO que puede recibir el patch

            patchDoc.ApplyTo(autorPatchDTO, ModelState); // Se aplican los cambios del documento patch al DTO, y se registra cualquier error en ModelState

            var esValido = TryValidateModel(autorPatchDTO); // Se valida que el modelo con los cambios cumpla con las reglas de validación

            if (!esValido) // Si el modelo no es válido después de aplicar el patch...
            {
                return ValidationProblem(); // ...se retorna un error 400 con los detalles de validación
            }

            mapper.Map(autorPatchDTO, autorDB); // Se mapea nuevamente el DTO al modelo original para actualizar sus valores con los cambios válidos

            await context.SaveChangesAsync(); // Se guardan los cambios en la base de datos de forma asincrónica

            return NoContent(); // Se retorna 204 (No Content), indicando que la operación fue exitosa pero no hay contenido que devolver
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(a => a.Id == id); //buscamos por identificaicon el autor
            if (autor == null) return NotFound(); //si es null retornamos no encontrado
            context.Remove(autor); //si el autor existe utilizar el dbContext borrar autor
            await context.SaveChangesAsync(); //guardamos cambios 
            await almacenadorArchivos.Borrar(autor.Foto, contenedor); //borramos la foto guardada en la ruta del autor y el contenedor
            return NoContent();
        }
    }
}
