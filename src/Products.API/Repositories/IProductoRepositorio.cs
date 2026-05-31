using Products.API.Models;

namespace Products.API.Repositories;

public interface IProductoRepositorio
{
    IEnumerable<Producto> ObtenerTodos();

    Producto? ObtenerPorId(string id);

    bool ExisteProductoConNombreYCategoria(string nombre, string categoria);

    void Crear(Producto producto);
}