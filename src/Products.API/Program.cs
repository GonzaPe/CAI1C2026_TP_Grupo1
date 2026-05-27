using Products.API.ExceptionHandlers;
using Products.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessRuleExceptionHandler>();

builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

app.UseExceptionHandler();

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

app.MapGet("/health/ready", () => Results.Ok(new
{
    status = "Healthy",
    service = "Products.API",
    check = "ready"
}));

app.MapGet("/health/live", () => Results.Ok(new
{
    status = "Healthy",
    service = "Products.API",
    check = "live"
}));

app.Run();



