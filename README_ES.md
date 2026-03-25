# Carnac w/ Mouse

**Visualizador de teclas y clics del mouse en tiempo real para Windows.**

> 🌐 [Read in English](README.md)

Carnac w/ Mouse muestra una capa transparente siempre visible que muestra cada tecla y clic del mouse conforme sucede. Es ideal para **presentaciones en vivo**, **screencasts**, **tutoriales** y **demos** donde tu audiencia necesita ver lo que estás escribiendo o haciendo clic.

![Windows](https://img.shields.io/badge/plataforma-Windows-blue)
![.NET 10](https://img.shields.io/badge/.NET-10.0-purple)
![WPF](https://img.shields.io/badge/UI-WPF-green)
![Licencia](https://img.shields.io/badge/licencia-MS--PL-orange)

## Características

- **Visualización de teclas** — Cada tecla presionada aparece en una capa flotante con tamaño, color, opacidad y tiempo de desvanecimiento configurables.
- **Indicadores de clic del mouse** — Círculos animados que se expanden para botón izquierdo, derecho, central y botones extra, cada uno con su propio color.
- **Visualización de scroll** — Opcionalmente muestra eventos de la rueda del mouse.
- **Soporte multi-monitor** — Elige en qué pantalla se muestra la capa y posiciónala en cualquier esquina con desplazamientos a nivel de píxel.
- **Detección de atajos** — Reconoce atajos de teclado de los keymaps incluidos (VS Code, Visual Studio, Chrome, ReSharper, NCrunch) y muestra sus nombres.
- **Soporte internacional de teclado** — Detecta automáticamente la distribución de teclado de Windows y muestra los caracteres correctos, incluyendo caracteres acentuados (á, é, í, ó, ú, ñ, ü, etc.) y secuencias de teclas muertas.
- **Modo contraseña / silencioso** — Presiona `Ctrl+Alt+P` para ocultar temporalmente todas las teclas (para ingresar contraseñas o datos sensibles). Presiona de nuevo para reanudar.
- **Filtrado por proceso** — Opcionalmente limita la visualización a aplicaciones específicas usando patrones regex.
- **Icono de aplicación** — Muestra el icono de la aplicación activa junto a las teclas.
- **Totalmente personalizable** — Más de 26 configuraciones para colores, tamaños, posiciones, animaciones y comportamiento.

## Requisitos

- Windows 10 o posterior
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (o .NET 10 SDK para compilar desde código fuente)

## Inicio Rápido

### Opción A: Ejecutar desde binario publicado

1. Descarga la última versión (o compílala tú mismo — ver abajo).
2. Ejecuta `w4b.carnac.exe`.
3. Aparece un icono en la bandeja del sistema — haz clic izquierdo para abrir las preferencias.
4. Comienza a escribir o hacer clic en cualquier lugar; la capa muestra tus teclas en tiempo real.

### Opción B: Compilar desde código fuente

```powershell
# Asegúrate de que .NET 10 SDK esté primero en PATH
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH

# Restaurar paquetes NuGet
dotnet restore src/w4b-carnac.sln -p:RestoreConfigFile="NuGet.Config"

# Compilar en modo Debug
dotnet build src/w4b-carnac.sln --no-restore

# Ejecutar la aplicación
dotnet run --project src/w4b.carnac/w4b.carnac.csproj

# O publicar ejecutable único en Release
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release
```

El binario publicado queda en `src/w4b.carnac/bin/Release/net10.0-windows/publish/w4b.carnac.exe`.

## Uso

| Acción | Cómo |
|--------|------|
| Abrir preferencias | Clic izquierdo en el icono de la bandeja |
| Salir | Clic derecho en el icono de la bandeja → **Exit** |
| Alternar modo silencioso | `Ctrl+Alt+P` |

Para una guía completa, consulta el **[Manual de Usuario (Español)](docs/MANUAL_USUARIO.md)** o el **[User Manual (English)](docs/USER_MANUAL.md)**.

## Configuración

Todas las configuraciones son accesibles desde la ventana de **Preferencias** (clic izquierdo en el icono de la bandeja):

| Pestaña | Configuraciones |
|---------|-----------------|
| **General** | Selección de pantalla, posición de la capa (esquina + desplazamientos) |
| **Keyboard** | Tamaño de fuente, color, fondo, opacidad, tiempo de desvanecimiento, modo solo atajos, modo solo modificadores, filtro de proceso |
| **Mouse** | Colores de clic (por botón), tamaño del indicador, escala de animación, borde, opacidad, tiempo de desvanecimiento, mostrar/ocultar clics y scroll |

## Estructura del Proyecto

| Proyecto | Descripción |
|----------|-------------|
| `w4b.carnac` | Aplicación WPF — UI, inyección de dependencias, icono de bandeja |
| `w4b.carnac.logic` | Capa de dominio — hooks de teclado/mouse, pipeline de mensajes, modelos |
| `w4b.carnac.tests` | Pruebas unitarias (xUnit + NSubstitute + Shouldly) |

Para detalles de arquitectura, consulta [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Historia y Reconocimientos

Este proyecto es una cadena de forks que preserva el trabajo de los autores originales:

1. **[Code52/carnac](https://github.com/Code52/carnac)** — Utilidad original Carnac para visualización de teclas.
2. **[bfritscher/carnac](https://github.com/bfritscher/carnac)** — Fork de Boris Fritscher que agrega resaltado de clics del mouse.
3. **[w4b-co-uk/carnac-w-mouse](https://github.com/w4b-co-uk/carnac-w-mouse)** — Actualizado a .NET 8 con limpieza de namespaces y trabajo multi-monitor. *(créditos al equipo de w4b)*
4. **[OscarTinajero117/carnac-w-mouse](https://github.com/OscarTinajero117/carnac-w-mouse)** — Modernizado a .NET 10, source generators de CommunityToolkit.Mvvm, logging estructurado con Serilog, soporte mejorado de teclados internacionales (corrección de teclas muertas) y documentación bilingüe completa.

## Licencia

[Microsoft Public License (MS-PL)](LICENSE.md)
