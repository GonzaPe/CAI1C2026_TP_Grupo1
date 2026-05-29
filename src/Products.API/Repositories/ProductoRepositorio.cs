using Dapper;
using Microsoft.Data.Sqlite;
using Products.API.Models;

namespace Products.API.Repositories;

public class ProductoRepositorio : IProductoRepositorio
{
    private readonly IConfiguration _configuracion;

    public ProductoRepositorio(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    private SqliteConnection CrearConexion()
    {
        string cadenaConexion = _configuracion.GetConnectionString("DefaultConnection")
            ?? "Data Source=products.db";

        return new SqliteConnection(cadenaConexion);
    }

    public IEnumerable<Producto> ObtenerTodos()
    {
        using var conexion = CrearConexion();

        string consulta = """
            SELECT
                id AS Id,
                nombre AS Nombre,
                descripcion AS Descripcion,
                precio AS Precio,
                stock AS Stock,
                categoria AS Categoria,
                fecha_creacion AS FechaCreacion
            FROM productos
            ORDER BY nombre;
        """;

        return conexion.Query<Producto>(consulta);
    }

    public Producto? ObtenerPorId(Guid id)
    {
        using var conexion = CrearConexion();

        string consulta = """
            SELECT
                id AS Id,
                nombre AS Nombre,
                descripcion AS Descripcion,
                precio AS Precio,
                stock AS Stock,
                categoria AS Categoria,
                fecha_creacion AS FechaCreacion
            FROM productos
            WHERE id = @Id;
        """;

        return conexion.QuerySingleOrDefault<Producto>(consulta, new
        {
            Id = id.ToString()
        });
    }

    public bool ExisteProductoConNombreYCategoria(string nombre, string categoria)
    {
        using var conexion = CrearConexion();

        string consulta = """
            SELECT COUNT(1)
            FROM productos
            WHERE lower(nombre) = lower(@Nombre)
              AND lower(categoria) = lower(@Categoria);
        """;

        int cantidad = conexion.ExecuteScalar<int>(consulta, new
        {
            Nombre = nombre,
            Categoria = categoria
        });

        return cantidad > 0;
    }

    public void Crear(Producto producto)
    {
        using var conexion = CrearConexion();

        string sentencia = """
            INSERT INTO productos (
                id,
                nombre,
                descripcion,
                precio,
                stock,
                categoria,
                fecha_creacion
            )
            VALUES (
                @Id,
                @Nombre,
                @Descripcion,
                @Precio,
                @Stock,
                @Categoria,
                @FechaCreacion
            );
        """;

        conexion.Execute(sentencia, new
        {
            Id = producto.Id.ToString(),
            producto.Nombre,
            producto.Descripcion,
            producto.Precio,
            producto.Stock,
            producto.Categoria,
            FechaCreacion = producto.FechaCreacion.ToString("O")
        });
    }
}