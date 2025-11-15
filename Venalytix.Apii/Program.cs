using Microsoft.OpenApi.Models; 
using Venalytix.Apication.Interfaces.IBase;
using Venalytix.Apication.DTOS.ClientesDTO;
using Venalytix.Apication.DTOS.ProductosDTO;
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


builder.Services.AddScoped<
    IRepositoryBase<SaveClienteDTO, UpdateClienteDTO, ObtenerClienteDTO>,
    ClienteRepository>();

builder.Services.AddScoped<
    IRepositoryBase<SaveProductoDTO, UpdateProductoDTO, ObtenerProductoDTO>,
    ProductoRepository>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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

