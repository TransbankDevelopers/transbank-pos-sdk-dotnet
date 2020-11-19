[![Build status](https://ci.appveyor.com/api/projects/status/y5tmyw3510dngbmh?svg=true)](https://ci.appveyor.com/project/TransbankDevelopers/transbank-pos-sdk-dotnet)
[![NuGet version](https://badge.fury.io/nu/TransbankPOSSDK.svg)](https://www.nuget.org/packages/TransbankPOSSDK)

# Transbank POS .Net SDK

SDK Oficial de Transbank para comunicarse con POS Verifone vx520 y vx520c

## Requisitos

- Visual Studio 2017+
- .NetFramework 4.6.1+
- .NetCore 2.0.3+

## Dependencias

- [libSerialPort](https://sigrok.org/wiki/Libserialport)
  - Puedes encontrar más información en [TransbankDevelopers](https://transbankdevelopers.cl/documentacion/posintegrado#libserialport)
- [TransbankWrap](https://github.com/TransbankDevelopers/transbank-pos-sdk-c)
  - Es un Wrapper de la librería en C para comunicarse con los POS.

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
dotnet add package TransbankSDK -v 2.1.0
```

### Desde Visual Studio

1. Abrir el explorador de soluciones.
2. Clic derecho en un proyecto dentro de tu solución.
3. Clic en Administrar paquetes NuGet.
4. Clic en la pestaña Examinar y marca el recuadro `Incluir PreReleases`
5. Buscar el paquete `TransbankPosSDK`.
6. Selecciona la versión que deseas utilizar y finalmente selecciona instalar.

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

### Generar una nueva versión

Para generar una nueva versión, se debe abrir un nuevo Pull Request, seguir la Guía de [SemVer](https://semver.org/) y empaquetar el artefacto NuGet:

1. Cambiar la configuración de la solución a Release.
2. Actualizar el numero de version.
3. Actualizar el Changelog con los cambios de la nueva version.
4. Click derecho sobre el proyecto.
5. Empaquetar.

Los pasos anteriores generaran un nuevo artefacto NuGet correspondiente a la versión modificada en la configuración.
Luego de generar esta nueva version, y mezclar el Pull Request con los cambios, es necesario la publicación manual en NugGet.

Web de [Nuget.org](https://www.nuget.org):

- Login en [Nuget.org](https://www.nuget.org)
- Upload
- Subir el artefacto NuGet generado anteriormente.

Linea de Comandos:

- Contar con ApiKey para poder subir el artefacto:

  ```bash
  dotnet.exe nuget push TransbankPosSDK/bin/Release/TransbankPosSDK.<version>.nupkg -k <APIKEY> -s https://api.nuget.org/v3/index.json
  ```

## Contribuciones ✨

Agradecimientos especiales a quienes nos ayudan a mejorar esta libreria.

<table>
  <tr>
    <td align="center"><a href="https://github.com/DarkFrostnight"><img src="https://avatars.githubusercontent.com/u/36648048?v=4" width="100px;" alt=""/><br /><sub><b>Mauricio Astudillo</b></sub></a><br /><a href="#bugs-DarkFrostnight" title="Reportar Bugs">🐛</a> <a href="userTest-DarkFrostnight" title="Pruebas de Usuario">📓</a> <a href="ideas-DarkFrostnight" title="Nuevas Ideas">🤔</a>
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
