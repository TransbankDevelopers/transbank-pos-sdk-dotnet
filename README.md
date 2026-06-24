[![Build status](https://ci.appveyor.com/api/projects/status/y5tmyw3510dngbmh?svg=true)](https://ci.appveyor.com/project/TransbankDevelopers/transbank-pos-sdk-dotnet)
[![NuGet version](https://badge.fury.io/nu/TransbankPOSSDK.svg)](https://www.nuget.org/packages/TransbankPOSSDK)

# Transbank POS .Net SDK

SDK Oficial de Transbank para comunicarse con equipos POS Integrado y POS Autoservicio:

- Verifone vx520
- Verifone vx520c
- Ingeniko Desk3500
- POS Autoservicio (UX100, UX300, UX400)
- POS Autoservicio IM30

## Requisitos

- Visual Studio 2017+
- .NetFramework 4.6.1+
- .NetCore 2.0.3+

## Dependencias

- [System.IO.Ports](https://www.nuget.org/packages/System.IO.Ports/)
  - Esta dependencia se instala automaticamente al usar NuGet, puedes encontrar más información en [el sitio del proyecto](https://github.com/dotnet/runtime)

## Instalación

### Desde una línea de comandos

#### Instalar con NuGet

```bash
nuget install TransbankPosSDK
```

#### Instalar con Package Manager

```bash
PM> Install-Package TransbankPosSDK
```

#### Instalar con .Net CLI

```bash
dotnet add package TransbankSDK -v 5.0.0
```

### Desde Visual Studio

1. Abrir el explorador de soluciones.
2. Clic derecho en un proyecto dentro de tu solución.
3. Clic en Administrar paquetes NuGet.
4. Buscar el paquete `TransbankPosSDK`.
5. Selecciona la versión que deseas utilizar y finalmente selecciona instalar.

## Documentación

Puedes encontrar toda la documentación de cómo usar este SDK en el sitio <https://www.transbankdevelopers.cl>.

La documentación relevante para usar este SDK es:

- Documentación general sobre el producto: [PosIntegrado](https://transbankdevelopers.cl/producto/posintegrado).
- [Primeros pasos](https://transbankdevelopers.cl/documentacion/posintegrado).
- [Referencia detallada](https://transbankdevelopers.cl/referencia/posintegrado).

## Información para contribuir y desarrollar este SDK

### Estándares

- Para los commits respetamos las siguientes normas: <https://chris.beams.io/posts/git-commit/>
- Usamos ingles, para los mensajes de commit.
- Se pueden usar tokens como WIP, en el subject de un commit, separando el token con `:`, por ejemplo: `WIP: This is a useful commit message`
- Para los nombres de ramas también usamos ingles.
- Se asume, que una rama de feature no mezclada, es un feature no terminado.
- El nombre de las ramas va en minúsculas.
- Las palabras se separan con `-`.
- Las ramas comienzan con alguno de los short lead tokens definidos, por ejemplo: `feat/tokens-configuration`

#### Short lead tokens

##### Commits

- WIP = Trabajo en progreso.

##### Ramas

- feat = Nuevos features
- chore = Tareas, que no son visibles al usuario.
- bug = Resolución de bugs.

### Todas las mezclas a master se hacen mediante Pull Request

### Construir el proyecto localmente

1. Si estas usando VisualStudio: (**F6**) o :
   - Click derecho sobre la solución en el explorador de soluciones.
   - Compilar.
2. Si estas usando tu propio editor:

   ```bash
   dotnet build
   ```

## Generar una nueva versión (con deploy automático a NuGet)

Para generar una nueva versión, se debe crear un PR (con un título "Prepare release X.Y.Z" con los valores que correspondan para `X`, `Y` y `Z`). Se debe seguir el estándar semver para determinar si se incrementa el valor de `X` (si hay cambios no retrocompatibles), `Y` (para mejoras retrocompatibles) o `Z` (si sólo hubo correcciones a bugs).

En ese PR deben incluirse los siguientes cambios:

1. Modificar al archivo `README.md` para que los ejemplos de instalación apunten a la ultima versión que se publicara.
2. Modificar el archivo `CHANGELOG.md` para incluir una nueva entrada (al comienzo) para `X.Y.Z` que explique en español los cambios **de cara al usuario del SDK**.
3. Modificar [TransbankPosSDK.csproj](./TransbankPosSDK/TransbankPosSDK.csproj) para que <`VersionPrefix`> sea `X.Y.{Z+1}` (de manera que los pre-releases que se generen después del release sean de la siguiente versión).

Luego de obtener aprobación del pull request, debe mezclarse a master e inmediatamente generar un release en GitHub con el tag `vX.Y.Z`. En la descripción del release debes poner lo mismo que agregaste al changelog.

Con eso Appveyor generará automáticamente una nueva versión de la librería y la publicará en NuGet.

## Contribuciones ✨

Agradecimientos especiales a quienes nos ayudan a mejorar esta librería.

<table>
  <tr>
    <td align="center"><a href="https://github.com/mastudillot"><img src="https://avatars.githubusercontent.com/u/36648048?v=4" width="100px;" alt=""/><br /><sub><b>Mauricio Astudillo</b></sub></a><br /><a href="#bugs-mastudillot" title="Reportar Bugs">🐛</a> <a href="userTest-mastudillot" title="Pruebas de Usuario">📓</a> <a href="ideas-mastudillot" title="Nuevas Ideas">🤔</a>
    </td>
  </tr>
</table>

### Simbología

<table>
  <tr>
    <td align="center">
      💻 <br> Código
    </td>
    <td align="center">
      📖 <br> Documentación
    </td>
       <td align="center">
      💡 <br> Ejemplos
    </td>
    </td>
       <td align="center">
      🤔 <br> Ideas
    </td>
  </tr>
    <tr>
    <td align="center">
      💬 <br> Preguntas
    </td>
    <td align="center">
      📓 <br> Pruebas de usuario
    </td>
       <td align="center">
      🐛 <br> Reporte de bugs
    </td>
    </td>
       <td align="center">
      👀 <br> Reviews
    </td>
  </tr>
</table>
