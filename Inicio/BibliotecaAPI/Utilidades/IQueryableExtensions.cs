using BibliotecaAPI.DTO;

namespace BibliotecaAPI.Utilidades
{
    public static class IQueryableExtensions
    {
        //Método de extensión genérico para cualquier tipo
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable,
            PaginacionDTO paginacionDTO)
        {
            return queryable
                //permite saltar un conjunto de registros
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina) //salta la paginacion 
                .Take(paginacionDTO.RecordsPorPagina); //toma los records
        }
    }
    //ejemplo del skip
    /*
     * .Skip((paginacionDTO.Pagina) = 2 
        menos 1 = 1 * 10 se salta los primeros 10 registros y toma los siguientes 10
     */
}
