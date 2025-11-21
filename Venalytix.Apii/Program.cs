using Microsoft.OpenApi.Models;
using Venalytix.Apication.DTOS.ClientesDTO;
using Venalytix.Apication.DTOS.ProductosDTO;
using Venalytix.Apication.Interfaces.IBase;
using Venalytix.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Venalytix API",
        Version = "v1",
        Description = "API para la gestión de Clientes y Productos (ETL AnalisisVentas)"
    });
});

// Registrar repositorios mediante fábricas que leen la cadena de conexión desde IConfiguration.
// Esto evita que el contenedor intente resolver un `string` directamente.
builder.Services.AddScoped<IRepositoryBase<SaveClienteDTO, UpdateClienteDTO, ObtenerClienteDTO>>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<ClienteRepository>>();
    var conn = config.GetConnectionString("DefaultConnection")
               ?? config["ConnectionStrings:DefaultConnection"];
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada. Comprueba appsettings.json.");
    return new ClienteRepository(conn, logger);
});

builder.Services.AddScoped<IRepositoryBase<SaveProductoDTO, UpdateProductoDTO, ObtenerProductoDTO>>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<ProductoRepository>>();
    var conn = config.GetConnectionString("DefaultConnection")
               ?? config["ConnectionStrings:DefaultConnection"];
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada. Comprueba appsettings.json.");
    return new ProductoRepository(conn, logger);
});

// Nota: no es necesario registrar IConfiguration explícitamente; el host ya lo hace.

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // ✅ Swagger solo en modo desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Venalytix API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();