# Decisiones tecnicas

## 1. Objetivo

Este documento registra las decisiones tecnicas tomadas durante el desarrollo del TP de microservicios eCommerce.

La finalidad es que el grupo pueda explicar y defender el codigo, las simplificaciones realizadas y el criterio seguido para mantener una solucion coherente.

## 2. Criterio general

El proyecto prioriza una implementacion:

- simple
- entendible
- defendible oralmente
- alineada con los documentos de la catedra
- consistente entre microservicios
- sin patrones avanzados innecesarios.

Se evita incorporar herramientas o arquitecturas que no hayan sido pedidas por el enunciado o mostradas por la catedra.

## 3. Framework y tipo de proyecto

Se utiliza:

```text
.NET 8
ASP.NET Core Web API
Controllers
```

La consigna indica .NET 8 y muestra una estructura basada en `Controllers`, por eso se eligio este enfoque en lugar de Minimal APIs.

## 4. Estructura por capas

Cada microservicio sigue el flujo:

```text
Controller
  ↓
Service
  ↓
Repository
  ↓
SQLite
```

Responsabilidades:

| Capa | Responsabilidad |
|---|---|
| `Controllers` | Recibir requests HTTP y devolver responses. |
| `DTOs` | Representar datos de entrada y salida de la API. |
| `Models` | Representar entidades internas del dominio. |
| `Services` | Concentrar reglas de negocio. |
| `Repositories` | Acceder a SQLite con Dapper. |
| `Data` | Crear tablas y datos iniciales. |
| `Exceptions` | Representar errores de dominio. |
| `ExceptionHandlers` | Convertir excepciones en respuestas HTTP estructuradas. |

## 5. Nombres y trazabilidad

Se mantiene una tendencia a usar nombres en español para clases, metodos y variables propios del grupo.

Ejemplos:

```text
Producto
ProductoServicio
ProductoRepositorio
SolicitudCrearProducto
RespuestaProducto
InicializadorBaseDatos
```

Se mantienen en ingles los nombres propios del framework, librerias, carpetas pedidas por la consigna y convenciones tecnicas:

```text
Controllers
Models
DTOs
Services
Repositories
Data
Exceptions
ExceptionHandlers
Program.cs
appsettings.json
Swagger
Health Checks
IExceptionHandler
IHttpClientFactory
```

Esta decision permite distinguir lo propio del dominio de lo propio del lenguaje o framework.

## 6. Persistencia

Se utiliza SQLite + Dapper.

SQLite permite tener una base local en un archivo `.db` por microservicio.

Dapper permite ejecutar SQL explicito desde los repositorios y mapear resultados a objetos C#.

Cada API tiene su propia base:

| API | Base |
|---|---|
| `Products.API` | `products.db` |
| `Users.API` | `users.db` |
| `Orders.API` | `orders.db` |
| `Cart.API` | `cart.db` |
| `Notifications.API` | `notifications.db` |

Los archivos `.db` son artefactos locales de ejecucion y no se versionan.

## 7. Inicializacion automatica de base

Cada microservicio tiene un inicializador en la carpeta `Data/`.

Al iniciar la API:

```text
Program.cs
  ↓
InicializadorBaseDatos
  ↓
CREATE TABLE IF NOT EXISTS
  ↓
Base lista para probar endpoints
```

Esto permite que el proyecto pueda ejecutarse sin crear manualmente las tablas.

## 8. Una base por microservicio

Se decidio usar una base SQLite por API para respetar la idea general de microservicios.

Un microservicio no deberia leer directamente la base de otro. Cuando necesita validar informacion externa, se comunica por HTTP con la API correspondiente.

Ejemplos:

```text
Orders.API -> Products.API
Orders.API -> Users.API
Cart.API -> Products.API
Notifications.API -> Users.API
```

## 9. Comunicacion HTTP entre servicios

Para llamadas entre microservicios se utiliza `IHttpClientFactory`, tal como indica la consigna.

Las URLs base de servicios externos se configuran desde `appsettings.json` o `appsettings.Development.json`.

Esto evita dejar URLs fijas dentro del codigo fuente.

## 10. Validacion de usuarios desde Orders.API

`Orders.API` incluye cliente HTTP para validar usuarios contra `Users.API`.

La validacion queda controlada por configuracion:

```json
{
  "ExternalServices": {
    "UsersApi": {
      "BaseUrl": "http://localhost:5223",
      "ValidationEnabled": true
    }
  }
}
```

Para la entrega final se recomienda usar `ValidationEnabled = true`, de manera que `Orders.API` pueda devolver `ORD-003` cuando el usuario no exista.

Durante desarrollo puede quedar deshabilitado temporalmente para probar Orders sin levantar Users, pero esa situacion debe estar documentada.

## 11. Endpoint tecnico en Users.API

`Users.API` incluye un endpoint tecnico:

```text
GET /api/users/{id}
```

Este endpoint no es parte principal de los casos funcionales de usuario, pero sirve para que otros microservicios validen la existencia de usuarios.

Se documenta como endpoint tecnico de integracion.

## 12. Validacion de productos y stock

`Orders.API` y `Cart.API` validan productos y stock consultando `Products.API`.

Esto permite demostrar comunicacion HTTP entre microservicios.

No se implementa una reserva formal de stock en carrito.

Tampoco se descuenta stock automaticamente al crear una orden, salvo que el codigo final lo implemente expresamente. La validacion principal es verificar disponibilidad al momento de crear orden o modificar carrito.

## 13. Borrado de productos con ordenes activas

La consigna pide que `Products.API` devuelva `PRD-004` cuando se intenta borrar un producto con ordenes activas.

Para poder demostrar ese caso en el TP, se utiliza una tabla local de apoyo que simula productos con ordenes activas.

Esta decision evita acoplar `Products.API` directamente a `Orders.API` para una validacion puntual y permite mostrar el error esperado desde Swagger.

Limitacion reconocida: en un sistema productivo real, esta regla deberia resolverse con un mecanismo de integracion mas robusto entre servicios.

## 14. Cart.API no valida Users.API

`Cart.API` valida:

- carrito existente;
- producto existente;
- stock suficiente;
- cantidad valida.

No valida usuario contra `Users.API`, porque el catalogo de errores de Cart no define un error especifico para usuario inexistente.

Se interpreta que `userId` identifica el carrito dentro del alcance de Cart.

## 15. Seguridad de passwords

`Users.API` no devuelve `PasswordHash` en ninguna respuesta.

Para el TP se implementa un hash simple para no guardar la contrasena en texto plano.

Limitacion reconocida: en un sistema real se usaria un mecanismo mas robusto con salt y algoritmos especificos para contrasenas.

## 16. Manejo de errores

Se implementa manejo global con `IExceptionHandler`.

Los servicios lanzan errores de dominio y los handlers construyen respuestas HTTP con el formato pedido:

```json
{
  "type": "...",
  "title": "...",
  "status": 404,
  "detail": "...",
  "instance": "...",
  "errorCode": "PRD-001",
  "errorMessage": "Producto no encontrado."
}
```

Esto evita que cada controller arme errores manualmente y mantiene consistencia entre APIs.

## 17. Logging

Se utiliza Serilog para logs estructurados.

Se registran eventos relevantes de request, errores de negocio y errores inesperados.

Los archivos de log se generan localmente y no se versionan en Git.

## 18. Correlation ID

Cada request utiliza `X-Correlation-Id`.

Criterio:

- si la request trae `X-Correlation-Id`, se reutiliza;
- si no lo trae, se genera uno;
- se agrega a logs;
- se propaga en llamadas HTTP salientes;
- se incluye en respuestas de error cuando corresponde.

Esto ayuda a seguir una request cuando intervienen varios microservicios.

## 19. Health checks

Cada microservicio expone:

```text
GET /health
GET /health/ready
GET /health/live
```

Estos endpoints permiten verificar que la API esta levantada y lista para recibir requests.

## 20. Swagger

Swagger se usa como herramienta principal de documentacion, prueba manual y demostracion.

Cada API debe mostrar:

- endpoints funcionales;
- request body;
- parametros;
- codigos HTTP;
- respuestas exitosas;
- respuestas de error con `errorCode` y `errorMessage`.

## 21. Archivos `.http`

Los archivos `.http` se mantienen como apoyo para pruebas desde Visual Studio.

Deben apuntar a endpoints reales del TP y no a `/weatherforecast`.

Si no se usan durante la defensa, siguen siendo utiles como documentacion tecnica y pruebas rapidas.

## 22. Pruebas

La consigna no exige pruebas automatizadas.

Se priorizan pruebas manuales documentadas desde Swagger, porque la defensa pide demo en vivo invocando endpoints exitosos y de error.

Si queda tiempo, se pueden agregar tests automatizados, pero no son el foco principal.

## 23. Artefactos no versionados

No deben subirse al repositorio:

```text
bin/
obj/
.vs/
*.db
*.db-wal
*.db-shm
*.log
*.user
```

Estos archivos son generados por Visual Studio, por la compilacion, por SQLite o por la ejecucion local.

## 24. Criterio de defensa

Cada decision debe poder explicarse de forma simple.

La defensa esperada es:

```text
Swagger recibe la request
  ↓
Controller llama al Service
  ↓
Service valida reglas de negocio
  ↓
Repository accede a SQLite con Dapper
  ↓
Se devuelve una respuesta HTTP correcta
```

El objetivo no es demostrar una arquitectura empresarial compleja, sino una implementacion completa, coherente y defendible del enunciado.
