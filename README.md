Trabajo práctico de Construcción de Aplicaciones Informáticas.

Integrantes
- Camila Albornoz
- Gonzalo Pérez
- Karen Gutierrez

Estructura
- src/: microservicios
- docs/: documentación, diagrama y capturas

ECommerce.sln
├── src/
│   ├── Products.API/
│   ├── Users.API/
│   ├── Orders.API/
│   ├── Cart.API/
│   └── Notifications.API/
├── docs/
└── README.md

Products.API/
  ├── Controllers/
  ├── Models/              # Entidades del dominio
  ├── DTOs/                # Request y Response DTOs
  ├── Services/            # Lógica de negocio
  ├── Exceptions/          # NotFoundException, BusinessRuleException, etc.
  ├── ExceptionHandlers/   # IExceptionHandler por tipo de excepción
  ├── logs/                # Archivos de log de Serilog
  └── Program.cs
