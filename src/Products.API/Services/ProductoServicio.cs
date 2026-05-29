using Products.API.DTOs;
using Products.API.Models;
using Products.API.Repositories;

namespace Products.API.Services;

public class ProductoServicio : IProductoServicio
{
    private readonly IProductoRepositorio _productoRepositorio;

    public ProductoServicio(IProductoRepositorio productoRepositorio)
    {
        _productoRepositorio = productoRepositorio;
    }

    public IEnumerable<RespuestaProducto> ObtenerTodos()
    {
        IEnumerable<Producto> productos = _productoRepositorio.ObtenerTodos();

        var respuestas = new List<RespuestaProducto>();

        foreach (Producto producto in productos)
        {
            respuestas.Add(ConvertirARespuesta(producto));
        }

        return respuestas;
    }

    public RespuestaProducto? ObtenerPorId(Guid id)
    {
        Producto? producto = _productoRepositorio.ObtenerPorId(id);

        if (producto == null)
        {
            return null;
        }

        return ConvertirARespuesta(producto);
    }

    public RespuestaProducto? Crear(SolicitudCrearProducto solicitud, out string? mensajeError)
    {
        mensajeError = null;

        if (string.IsNullOrWhiteSpace(solicitud.Nombre))
        {
            mensajeError = "El nombre del producto es obligatorio.";
            return null;
        }

        if (solicitud.Precio <= 0)
        {
            mensajeError = "El precio debe ser mayor a cero.";
            return null;
        }

        if (solicitud.Stock < 0)
        {
            mensajeError = "El stock no puede ser negativo.";
            return null;
        }

        if (string.IsNullOrWhiteSpace(solicitud.Categoria))
        {
            mensajeError = "La categoria es obligatoria.";
            return null;
        }

        bool productoDuplicado = _productoRepositorio.ExisteProductoConNombreYCategoria(
            solicitud.Nombre,
            solicitud.Categoria);

        if (productoDuplicado)
        {
            mensajeError = "Ya existe un producto con ese nombre en la categoria indicada.";
            return null;
        }

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = solicitud.Nombre,
            Descripcion = solicitud.Descripcion,
            Precio = solicitud.Precio,
            Stock = solicitud.Stock,
            Categoria = solicitud.Categoria,
            FechaCreacion = DateTime.UtcNow
        };

        _productoRepositorio.Crear(producto);

        return ConvertirARespuesta(producto);
    }

    private RespuestaProducto ConvertirARespuesta(Producto producto)
    {
        return new RespuestaProducto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock,
            Categoria = producto.Categoria,
            FechaCreacion = producto.FechaCreacion
        };
    }
}