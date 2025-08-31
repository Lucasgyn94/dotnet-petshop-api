using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Autenticação JWT
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = false,
                        ValidateAudience         = false,
                        ValidateLifetime         = true,
                        IssuerSigningKey         = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["jwt"]!))
                    };
                });

            // 2) Autorização
            builder.Services.AddAuthorization();

            // 3) Serviços de aplicação
            builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
            builder.Services.AddScoped<ITokenServico, TokenServico>();

            // 4) DbContext
            var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DbContexto>(options =>
                options.UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao))
            );

            // 5) CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            // 6) Swagger / OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title       = "API de gerenciamento de Pets",
                    Version     = "v1",
                    Description = "Minimal API .NET 8 com JWT, Clean Architecture e Swagger"
                });

                // Define o esquema de segurança JWT no Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    Type         = SecuritySchemeType.Http,
                    Scheme       = "bearer",
                    BearerFormat = "JWT",
                    In           = ParameterLocation.Header,
                    Description  = "Insira o token usando Bearer {seu_token}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            var app = builder.Build();

            // 7) Pipeline de middlewares
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            // 8) Mapeia endpoints
            app.MapAdministradorEndpoints();

            app.Run();
        }
    }
}
