using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Utilidades
{
    public static class HttpContextExtensions
    {
        //la cantidad de autores se manda en la cabecera de la respuesta 
        public async static Task 
            //
            InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if(httpContext is null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Append("cantidad-total-registros", cantidad.ToString());
        } 
    }
}
//Añade un encabezado (header) HTTP a la respuesta llamada "cantidad-total-registros"
//con el valor del total de registros.
/*
Esto sirve para que el cliente (front-end o consumidor API) pueda 
saber cuántos registros existen en total, útil para paginación.
se debe ir a program y mandarlo a cabeceras expuestas
*/