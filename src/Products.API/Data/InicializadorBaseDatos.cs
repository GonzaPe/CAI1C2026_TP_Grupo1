using Dapper;
using Microsoft.Data.Sqlite;

namespace Products.API.Data;

public class InicializadorBaseDatos
{
    private readonly IConfiguration _configuracion;

    public InicializadorBaseDatos(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public void Inicializar()
    {
        string cadenaConexion = _configuracion.GetConnectionString("DefaultConnection")
            ?? "Data Source=products.db";

        using var conexion = new SqliteConnection(cadenaConexion);

        conexion.Open();

        // Crea la tabla si todavía no existe.
        conexion.Execute("""
            CREATE TABLE IF NOT EXISTS productos (
                id TEXT PRIMARY KEY,
                nombre TEXT NOT NULL,
                descripcion TEXT NULL,
                precio REAL NOT NULL,
                stock INTEGER NOT NULL,
                categoria TEXT NOT NULL,
                fecha_creacion TEXT NOT NULL
            );
        """);

        // Carga inicial mínima para poder probar un GET sin tener que hacer antes un POST.
        int cantidadProductos = conexion.ExecuteScalar<int>("SELECT COUNT(1) FROM productos;");

        if (cantidadProductos == 0)
        {
            conexion.Execute("""
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
                    '3fa85f64-5717-4562-b3fc-2c963f66afa6',
                    'Notebook Dell XPS 15',
                    'Laptop 15 pulgadas, 32GB RAM',
                    1500.00,
                    10,
                    'Electronica',
                    datetime('now')
                );
            """);

            conexion.Execute("""
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
                    'aaaabbbb-cccc-dddd-eeee-ffff00001111',
                    'Mouse Logitech',
                    'Mouse inalambrico',
                    25.50,
                    30,
                    'Electronica',
                    datetime('now')
                );
            """);
        }
    }
}