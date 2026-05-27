using Products.API.DTOs;

namespace Products.API.Services;

public interface IProductService
{
    IEnumerable<ProductResponse> GetAll(string? categoria, string? nombre);
    ProductResponse GetById(Guid id);
    ProductResponse Create(CreateProductRequest request);
}