using Products.API.DTOs;
using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Nombre = "Notebook Dell XPS 15",
            Descripcion = "Laptop 15 pulgadas, 32GB RAM",
            Precio = 1500.00m,
            Stock = 10,
            Categoria = "Electrónica",
            FechaCreacion = DateTime.UtcNow
        },
        new Product
        {
            Id = Guid.Parse("aaaabbbb-cccc-dddd-eeee-ffff00001111"),
            Nombre = "Mouse Logitech",
            Descripcion = "Mouse inalámbrico",
            Precio = 25.50m,
            Stock = 30,
            Categoria = "Electrónica",
            FechaCreacion = DateTime.UtcNow
        }
    };

    public IEnumerable<ProductResponse> GetAll(string? categoria, string? nombre)
    {
        IEnumerable<Product> query = _products;

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(p =>
                p.Categoria.Contains(categoria, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(p =>
                p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
        }

        return query.Select(ToResponse);
    }

    public ProductResponse GetById(Guid id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }

        return ToResponse(product);
    }

    public ProductResponse Create(CreateProductRequest request)
    {
        var duplicated = _products.Any(p =>
            p.Nombre.Equals(request.Nombre, StringComparison.OrdinalIgnoreCase)
            && p.Categoria.Equals(request.Categoria, StringComparison.OrdinalIgnoreCase));

        if (duplicated)
        {
            throw new BusinessRuleException(
                "PRD-003",
                $"Ya existe un producto con ese nombre en la categoría '{request.Categoria}'.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Precio = request.Precio,
            Stock = request.Stock,
            Categoria = request.Categoria,
            FechaCreacion = DateTime.UtcNow
        };

        _products.Add(product);

        return ToResponse(product);
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Nombre = product.Nombre,
            Descripcion = product.Descripcion,
            Precio = product.Precio,
            Stock = product.Stock,
            Categoria = product.Categoria,
            FechaCreacion = product.FechaCreacion
        };
    }
}
