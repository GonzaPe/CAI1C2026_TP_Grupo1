using Products.API.DTOs;

namespace Products.API.Services;

public interface IProductoServicio
{
    IEnumerable<RespuestaProducto> ObtenerTodos();

    RespuestaProducto? ObtenerPorId(Guid id);

    RespuestaProducto? Crear(SolicitudCrearProducto solicitud, out string? mensajeError);
}