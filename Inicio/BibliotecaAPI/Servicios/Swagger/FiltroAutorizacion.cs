using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BibliotecaAPI.Servicios.Swagger
{
    public class FiltroAutorizacion : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //valida si en la descripcion existe un metadata que una cabecera [Authorize]
            if (!context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>().Any())
            {
                return; //va a retornar por que no esta protegido
            }
            //valida que en la descripcion tenga metadata con un [AllowAnonymous]
            if (context.ApiDescription.ActionDescriptor
                .EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                         new string[]{}
                    }

                }
            };
        }
    }
}
