using Products.API.Data;
using Products.API.Repositories;
using Products.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<InicializadorBaseDatos>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<IProductoServicio, ProductoServicio>();

var app = builder.Build();

// Al arrancar la API, crea la base y la tabla si no existen.
using (var scope = app.Services.CreateScope())
{
    var inicializador = scope.ServiceProvider.GetRequiredService<InicializadorBaseDatos>();
    inicializador.Inicializar();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "Products.API"
}));

app.Run();