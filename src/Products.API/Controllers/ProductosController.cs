using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Services;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductosController : ControllerBase
{
    private readonly IProductoServicio _productoServicio;

    public ProductosController(IProductoServicio productoServicio)
    {
        _productoServicio = productoServicio;
    }

    [HttpGet]
    public ActionResult<IEnumerable<RespuestaProducto>> ObtenerTodos()
    {
        IEnumerable<RespuestaProducto> productos = _productoServicio.ObtenerTodos();

        return Ok(productos);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<RespuestaProducto> ObtenerPorId(Guid id)
    {
        RespuestaProducto? producto = _productoServicio.ObtenerPorId(id);

        if (producto == null)
        {
            return NotFound(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = "Not Found",
                status = 404,
                detail = "El recurso solicitado no fue encontrado.",
                instance = HttpContext.Request.Path.Value,
                errorCode = "PRD-001",
                errorMessage = "Producto no encontrado."
            });
        }

        return Ok(producto);
    }

    [HttpPost]
    public ActionResult<RespuestaProducto> Crear(SolicitudCrearProducto solicitud)
    {
        RespuestaProducto? productoCreado = _productoServicio.Crear(
            solicitud,
            out string? mensajeError);

        if (productoCreado == null)
        {
            return BadRequest(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "Bad Request",
                status = 400,
                detail = "Los datos del producto son invalidos.",
                instance = HttpContext.Request.Path.Value,
                errorCode = "PRD-002",
                errorMessage = mensajeError
            });
        }

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = productoCreado.Id },
            productoCreado);
    }
}