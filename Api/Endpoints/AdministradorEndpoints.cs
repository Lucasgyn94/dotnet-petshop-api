using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Api;

public static class AdministradorEndpoints
{
    public static void MapAdministradorEndpoints(this IEndpointRouteBuilder app)
    {
        var grupoAdministrador = app.MapGroup("/administradores").WithTags("Administradores");

        grupoAdministrador.MapGet("/", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
        {
            List<Administrador> administradores = administradorServico.Todos(pagina);

            IEnumerable<AdministradorModelView> administradorModelView = administradores.Select(administrador => new AdministradorModelView
            {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });

            return Results.Ok(administradorModelView);

        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });

        grupoAdministrador.MapGet("{id}", (int id, IAdministradorServico administradorServico) =>
        {
            var administrador = administradorServico.BuscarPorId(id);

            if (administrador != null)
            {
                return Results.Ok(new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            }
            else
            {
                return Results.NotFound();
            }
        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });


        grupoAdministrador.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico, ITokenServico tokenServico) =>
        {
            var administrador = administradorServico.Login(loginDTO);

            if (administrador is not null)
            {
                string token = tokenServico.GerarToken(administrador);

                return Results.Ok(new AdministradorLogado
                {
                    Email = administrador.Email,
                    Perfil = administrador.Perfil,
                    Token = token
                });
            }
            else
            {
                return Results.Unauthorized();
            }
        }).AllowAnonymous();

        grupoAdministrador.MapPost("/", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
        {
            var administrador = new Administrador
            {
                Email = administradorDTO.Email,
                Senha = administradorDTO.Senha,
                Perfil = administradorDTO.Perfil
            };

            var administradorIncluido = administradorServico.Incluir(administrador);

            var administradorModelView = new AdministradorModelView
            {
                Id = administradorIncluido.Id,
                Email = administradorIncluido.Email,
                Perfil = administradorIncluido.Perfil
            };

            return Results.Created($"/administrador/{administradorIncluido.Id}", administradorModelView);

        }).RequireAuthorization(new AuthorizeAttribute
        {
            Roles = "Adm"
        });

    
    }

}
