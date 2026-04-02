# Carnac w/ Mouse — Manual de Usuario

> 🌐 [Read in English](USER_MANUAL.md)

## Tabla de Contenidos

1. [Introducción](#introducción)
2. [Instalación](#instalación)
3. [Primer Inicio](#primer-inicio)
4. [El Icono de la Bandeja del Sistema](#el-icono-de-la-bandeja-del-sistema)
5. [Ventana de Preferencias](#ventana-de-preferencias)
   - [Pestaña General](#pestaña-general)
   - [Pestaña Keyboard (Teclado)](#pestaña-keyboard-teclado)
   - [Pestaña Mouse (Ratón)](#pestaña-mouse-ratón)
   - [Pestaña About (Acerca de)](#pestaña-about-acerca-de)
6. [Modo Silencioso (Protección de Contraseñas)](#modo-silencioso-protección-de-contraseñas)
7. [Soporte de Teclados Internacionales](#soporte-de-teclados-internacionales)
8. [Reconocimiento de Atajos](#reconocimiento-de-atajos)
9. [Archivos de Log](#archivos-de-log)
10. [Solución de Problemas](#solución-de-problemas)

---

## Introducción

Carnac w/ Mouse es un **visualizador en tiempo real de teclas y clics del mouse** para Windows. Muestra una capa transparente siempre visible que presenta cada tecla que presionas y cada botón del mouse en que haces clic. Es perfecto para:

- **Presentaciones en vivo** — Tu audiencia puede ver los atajos de teclado mientras los usas.
- **Screencasts y tutoriales** — Los espectadores pueden seguir exactamente las teclas que presionas.
- **Aprender atajos de teclado** — Carnac detecta atajos conocidos y muestra sus nombres.
- **Demos** — Muestra exactamente lo que estás haciendo sin necesidad de explicación verbal.

---

## Instalación

### Requisitos Previos

- **Sistema Operativo**: Windows 10 o posterior.
- **Runtime**: [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).

### Pasos

1. Descarga la última versión de `w4b.carnac.exe` (ejecutable de archivo único).
2. Colócalo en cualquier lugar de tu computadora (por ejemplo, `C:\Herramientas\Carnac\`).
3. Haz doble clic en `w4b.carnac.exe` para iniciar.

> **Consejo**: Haz clic derecho en el ejecutable → **Crear acceso directo** y colócalo en tu carpeta de Inicio (`shell:startup`) para que Carnac se lance automáticamente con Windows.

---

## Primer Inicio

Cuando ejecutas Carnac por primera vez:

1. La aplicación inicia **minimizada** — no hay ventana principal.
2. Un pequeño icono aparece en la **bandeja del sistema** (parte inferior derecha de la barra de tareas).
3. Una capa transparente se activa en una de tus pantallas.
4. Comienza a escribir o hacer clic en cualquier lugar — tus teclas y clics del mouse aparecen en la capa inmediatamente.

---

## El Icono de la Bandeja del Sistema

El icono de la bandeja del sistema es la forma principal de interactuar con Carnac:

| Acción | Resultado |
|--------|-----------|
| **Clic izquierdo** | Abre la ventana de Preferencias |
| **Clic derecho** | Muestra menú contextual con la opción **Exit** (Salir) |

---

## Ventana de Preferencias

Haz clic izquierdo en el icono de la bandeja para abrir la ventana de Preferencias. Tiene cuatro pestañas:

### Pestaña General

Controla **dónde** aparece la capa.

| Configuración | Descripción |
|---------------|-------------|
| **Selector de pantalla** | Representación visual de todos los monitores conectados. Haz clic en uno para elegir en qué pantalla se muestra la capa. |
| **Posición de notificación** | Elige una esquina: Arriba-Izquierda, Arriba-Derecha, Abajo-Izquierda o Abajo-Derecha. |
| **Top Offset** | Distancia en píxeles desde el borde superior de la pantalla. |
| **Bottom Offset** | Distancia en píxeles desde el borde inferior de la pantalla. |
| **Left Offset** | Distancia en píxeles desde el borde izquierdo de la pantalla. |
| **Right Offset** | Distancia en píxeles desde el borde derecho de la pantalla. |
| **Language** (Idioma) | Selecciona el idioma de la interfaz: English (por defecto), Español o Português (Brasil). Los cambios se aplican inmediatamente en la ventana de Preferencias. |

### Pestaña Keyboard (Teclado)

Controla cómo aparecen las **teclas** en la capa.

| Configuración | Descripción | Valor predeterminado |
|---------------|-------------|----------------------|
| **Popup Text Width** | Ancho máximo en píxeles del área de visualización de teclas. | 350 |
| **Popup Opacity** | Transparencia del texto. 0 = invisible, 1 = completamente opaco. | 0.5 |
| **Popup Fade Delay** | Segundos antes de que la tecla se desvanezca. | 5 |
| **Font Size** | Tamaño del texto en píxeles (rango: 8–48). | 40 |
| **Font Colour** | Color del texto (cualquier nombre de color de Windows, ej: "White", "Cyan", "Yellow"). | White |
| **Background Color** | Color de fondo detrás del texto. | Black |
| **Shortcuts Only** | Cuando está activado, solo muestra combinaciones de teclas encontradas en los archivos de keymaps. La escritura normal se oculta. | Desactivado |
| **Custom Keymaps** | Ruta a una carpeta opcional con archivos `.yml` de keymaps adicionales. Usa el botón Browse para seleccionar la carpeta. Se aplica al reiniciar. | Vacío |
| **Only Keys with Modifiers** | Cuando está activado, solo muestra combinaciones que incluyen Ctrl, Alt, Shift o Win. La escritura normal se oculta. | Desactivado |
| **Show Space as ␣** | Muestra la tecla de espacio como el símbolo Unicode de caja abierta en lugar de un espacio en blanco. | Desactivado |
| **Show Application Icon** | Muestra el icono de la aplicación activa junto a la tecla presionada. | Desactivado |
| **Process Filter** | Patrón regex para limitar la visualización a aplicaciones específicas. Déjalo vacío para mostrar todo. Ejemplo: `chrome|firefox` para solo mostrar teclas en navegadores. | Vacío |

### Pestaña Mouse (Ratón)

Controla cómo aparecen los **clics del mouse** en la capa.

| Configuración | Descripción | Valor predeterminado |
|---------------|-------------|----------------------|
| **Show Mouse Clicks** | Activa/desactiva la visualización de clics del mouse. | Activado |
| **Show Clicks as Keys** | Muestra los nombres de los clics (ej: "LButton") en el área de teclas de la capa. | Activado |
| **Show Scroll as Keys** | Muestra eventos de la rueda del mouse en la capa. | Activado |
| **Mouse Key Size** | Tamaño del círculo indicador de clic en píxeles (8–300). | 40 |
| **Start Scale** | Multiplicador de tamaño inicial de la animación del clic. | 1 |
| **Stop Scale** | Multiplicador de tamaño final de la animación del clic (crea efecto de expansión). | 4 |
| **Circle Fade Delay** | Milisegundos antes de que el indicador de clic desaparezca (100–5000). | 3700 |
| **Start Border** | Ancho inicial del borde del círculo indicador. | 1 |
| **Start Opacity** | Transparencia inicial del indicador (0–1). | 0.8 |
| **Stop Border** | Ancho final del borde del indicador. | 2 |
| **Stop Opacity** | Transparencia final del indicador (0 = completamente desvanecido). | 0 |
| **Left Click Color** | Color del indicador del botón izquierdo del mouse. | OrangeRed |
| **Right Click Color** | Color del indicador del botón derecho del mouse. | RoyalBlue |
| **Scroll Click Color** | Color del indicador de la rueda del mouse. | Gold |
| **XButton1 Click Color** | Color del indicador del botón 4 del mouse (botón lateral). | Peru |
| **XButton2 Click Color** | Color del indicador del botón 5 del mouse (botón lateral). | Plum |

### Pestaña About (Acerca de)

Muestra créditos, versiones de componentes y enlaces al repositorio del proyecto.

---

## Modo Silencioso (Protección de Contraseñas)

Cuando necesites escribir una contraseña u otra información sensible:

1. Presiona **`Ctrl+Alt+P`** — la capa deja de mostrar teclas y clics del mouse.
2. Escribe tu contraseña o datos sensibles normalmente.
3. Presiona **`Ctrl+Alt+P`** de nuevo — la capa reanuda la operación normal.

> **Importante**: El modo silencioso es un interruptor. Debes presionar el atajo una segunda vez para volver a habilitar la visualización de teclas.

---

## Soporte de Teclados Internacionales

Carnac detecta automáticamente la distribución de teclado activa de Windows y muestra los caracteres correctos para tu idioma. Esto significa:

- **Teclados en español**: `ñ`, vocales acentuadas (`á`, `é`, `í`, `ó`, `ú`) y `ü` se muestran correctamente.
- **AZERTY francés**: Los caracteres se mapean a sus posiciones físicas correctas.
- **QWERTZ alemán**: Las diéresis (`ä`, `ö`, `ü`) y `ß` se muestran correctamente.
- **Cualquier otra distribución de teclado de Windows**: Los caracteres se resuelven a través de la API `ToUnicodeEx` de Windows, por lo que cualquier distribución soportada por Windows funcionará.

### Teclas Muertas (Marcas de Acento)

Las teclas muertas son teclas que no producen un carácter inmediatamente sino que modifican la siguiente tecla presionada (por ejemplo, presionar `´` y luego `a` produce `á` en teclados en español).

Carnac maneja las teclas muertas de la siguiente forma:
1. Muestra la marca del acento cuando se presiona la tecla muerta.
2. Muestra el siguiente carácter de forma normal.
3. Preserva el estado de la tecla muerta para que la aplicación destino siga recibiendo el carácter combinado correcto.

### Cambiar Distribución de Teclado

Puedes cambiar tu distribución de teclado en Windows presionando **`Win+Espacio`**. Carnac detecta el cambio automáticamente y comienza a usar la nueva distribución inmediatamente.

---

## Reconocimiento de Atajos

Carnac incluye archivos de keymaps para aplicaciones populares:

| Keymap | Aplicación |
|--------|------------|
| `chrome.yml` | Google Chrome |
| `visual-studio.yml` | Visual Studio IDE |
| `vscode.yml` | Visual Studio Code |
| `resharper.yml` | ReSharper |
| `ncrunch.yml` | NCrunch |

Cuando presionas una combinación de teclas que coincide con un atajo en estos archivos y la aplicación correspondiente está en primer plano, Carnac muestra el **nombre del atajo** junto con las teclas.

Estos archivos de keymaps se encuentran en la carpeta `Keymaps/` junto al ejecutable y usan formato YAML. Puedes editarlos o agregar nuevos.

### Formato de Keymap

```yaml
group: nombre-de-la-aplicacion
process: nombre-del-proceso    # Nombre del ejecutable (sin .exe)

shortcuts:
  - name: Nombre descriptivo del atajo
    keys:
      - Ctrl+C                  # Combinación de teclas
  - name: Otro atajo
    keys:
      - Ctrl+Shift+P            # Combinación principal
      - F1                      # Combinación alternativa
```

---

## Archivos de Log

Carnac escribe logs de diagnóstico en:

```
%LOCALAPPDATA%\Carnac\logs\carnac-AAAAMMDD.log
```

Para abrir esta carpeta, presiona `Win+R`, escribe la ruta arriba y presiona Enter.

Los logs rotan diariamente y se conservan durante 7 días. Usa estos archivos para diagnosticar problemas.

---

## Solución de Problemas

### La capa no aparece

- Asegúrate de que Carnac esté ejecutándose (busca el icono en la bandeja del sistema).
- Abre Preferencias → pestaña General y verifica que el monitor correcto esté seleccionado.
- Intenta cambiar la posición de notificación a una esquina diferente.

### Las teclas no se muestran

- Asegúrate de que el modo silencioso no esté activo (presiona `Ctrl+Alt+P` para alternar).
- Verifica si "Shortcuts Only" o "Only Keys with Modifiers" está activado en la pestaña Keyboard.
- Revisa el Process Filter — si tiene un valor, Carnac solo muestra teclas de las aplicaciones que coincidan.

### Los caracteres acentuados no se muestran correctamente

- Verifica que tu distribución de teclado de Windows esté configurada para tu idioma (ej: "Español (España)" o "Español (Latinoamérica)").
- Cambia a tu distribución deseada usando `Win+Espacio` en Windows.
- Carnac usa tu distribución de teclado activa; los cambios surten efecto inmediatamente.

### La capa bloquea los clics del mouse

- La capa está diseñada para permitir clics a través de ella — los clics pasan a las aplicaciones debajo. Si esto no funciona, intenta reiniciar Carnac.

### Los indicadores de clic del mouse no aparecen

- Abre Preferencias → pestaña Mouse y asegúrate de que "Show Mouse Clicks" esté activado.
- Verifica que el Circle Fade Delay no sea muy corto (auméntalo para ver el efecto).
- Verifica que Start Opacity esté por encima de 0.

### La aplicación no inicia

- Verifica que tengas instalado el [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0).
- Revisa los logs en `%LOCALAPPDATA%\Carnac\logs\` para ver mensajes de error detallados.
- Intenta ejecutar la aplicación desde una terminal para ver errores de consola:
  ```powershell
  .\w4b.carnac.exe
  ```

## Debug

```powershell
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH
dotnet run --project src/w4b.carnac/w4b.carnac.csproj
```

## Compilar para Release

Para compilar una versión de release de Carnac:

```powershell
# 1. Asegúrate de que .NET 10 SDK esté primero en PATH
$env:PATH = "C:\Program Files\dotnet;" + $env:PATH

# 2. Restaurar paquetes NuGet (usa NuGet.Config local para evitar config offline de VS)
dotnet restore src/w4b-carnac.sln -p:RestoreConfigFile="NuGet.Config"

# 3. Compilar en modo Release
dotnet build src/w4b-carnac.sln -c Release --no-restore

# 4. Ejecutar pruebas para verificar que todo pase
dotnet test src/w4b.carnac.tests/w4b.carnac.tests.csproj -c Release --no-build --nologo

# 5. Publicar ejecutable único
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release --no-restore
```

El resultado es un ejecutable único en:

```
src/w4b.carnac/bin/Release/net10.0-windows/publish/w4b.carnac.exe
```

Este archivo depende del framework (requiere .NET 10 Desktop Runtime en la máquina destino). Para crear un ejecutable completamente autocontenido:

```powershell
dotnet publish src/w4b.carnac/w4b.carnac.csproj -c Release --self-contained true -r win-x64
```

El ejecutable autocontenido queda en `src/w4b.carnac/bin/Release/net10.0-windows/win-x64/publish/w4b.carnac.exe`.