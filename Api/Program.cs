using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Api.Datos;
using Api.Endpoints;
using Api.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Servicios base
// ----------------------------------------------------
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });

    // Definición de seguridad: Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingresa: Bearer {tu_token}"
    });

    // Requerimiento global de seguridad
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ----------------------------------------------------
// Base de datos (MySQL)
// ----------------------------------------------------
string? cs = builder.Configuration.GetConnectionString("ConexionPrincipal")
             ?? Environment.GetEnvironmentVariable("ConnectionStrings__ConexionPrincipal");

if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("No hay cadena de conexión 'ConexionPrincipal'.");

builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// ----------------------------------------------------
// Servicios de dominio (TODOS Scoped)
// ----------------------------------------------------
builder.Services.AddScoped<ServicioConfiguraciones>();   // ← IMPORTANTE: Scoped
builder.Services.AddScoped<ServicioEmail>();
builder.Services.AddScoped<ServicioReportes>();
builder.Services.AddScoped<ServicioJwt>();

// ----------------------------------------------------
// Autenticación JWT
// ----------------------------------------------------
string emisor = builder.Configuration["Jwt:Emisor"]
                ?? Environment.GetEnvironmentVariable("Jwt__Emisor")
                ?? "VisitasEmisor";

string audiencia = builder.Configuration["Jwt:Audiencia"]
                  ?? Environment.GetEnvironmentVariable("Jwt__Audiencia")
                  ?? "VisitasAudiencia";

string claveJwt = builder.Configuration["Jwt:Clave"]
                 ?? Environment.GetEnvironmentVariable("Jwt__Clave")
                 ?? "CLAVE_DEMO_SUPER_SECRETA_CAMBIAR_32+_CARACTERES";

var seguridad = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = emisor,
            ValidAudience = audiencia,
            IssuerSigningKey = seguridad,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// ----------------------------------------------------
// SOLO DESARROLLO: ignorar errores de certificado TLS
// (por ejemplo, para smtp.gmail.com dentro del contenedor).
// IMPORTANTE: quitar o ajustar esto en producción/AWS.
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    ServicePointManager.ServerCertificateValidationCallback +=
        (object? sender,
         X509Certificate? certificate,
         X509Chain? chain,
         SslPolicyErrors sslPolicyErrors) =>
        {
            return true;
        };
}

// ----------------------------------------------------
// Crear BD + semilla
// ----------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    Semilla.DatosIniciales(db);
}

// ----------------------------------------------------
// Pipeline HTTP
// ----------------------------------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
});

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------------------------------
// Endpoints
// ----------------------------------------------------
AutenticacionEndpoints.Mapear(app);
ClientesEndpoints.Mapear(app);
UsuariosEndpoints.Mapear(app);
VisitasEndpoints.Mapear(app);
ConfigEndpoints.Mapear(app);
ReportesEndpoints.Mapear(app);

app.Run();
