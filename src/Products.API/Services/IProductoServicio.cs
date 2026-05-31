using Products.API.DTOs;

namespace Products.API.Services;

public interface IProductoServicio
{
    IEnumerable<RespuestaProducto> ObtenerTodos();

    RespuestaProducto? ObtenerPorId(string id);

    RespuestaProducto? Crear(SolicitudCrearProducto solicitud, out string? mensajeError);
}