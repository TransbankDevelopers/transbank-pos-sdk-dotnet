# Changelog

Todos los cambios notables a este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
y este proyecto adhiere a [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.3.0] - 2020-11-19

Se elimina el uso de las antiguas librerias en C, en favor de una libreria nativa de C# que mantiene la compatibilidad multiplataforma.

### Added

- Soporte para nuevos equipos Ingenico Desk3500.
- Soporte para venta multi codigo en el metodo `MultiCodeSale`.
  - Nuevo objeto de respuesta `MultiCodeDetails`.
  - Nueva excepción `TransbankMultiCodeSaleException`.
- Soporte para rescatar la ultima venta multicodigo en el metodo `MultiCodeLastSale`.
  - Nuevo objeto de respuesta `MultiCodeLastSaleResponse`.
  - Nueva excepción `TransbankMultiCodeLastSaleException`.
- Soporte para rescatar el detalle de ventas multicodigo en el metodo `MultiCodeDetails`.
  - Nuevo objeto de respuesta `MultiCodeDetailResponse`.
  - Nueva excepción `TransbankMultiCodeDetailException`.

### Changed

- Se elimina la dependecia del Wrapper en C y de Libserialport, reemplazandola por el uso de la libreria Nuget `System.IO.Ports`.

## [2.1.2] - 2020-03-24

### Fix

- Bug en `DetailResponse`, el número de ticket se estaba parseando incorrectamente cuando contenía letras.
- Bug en `LastSale`, el número de ticket se estaba parseando incorrectamente cuando contenía letras.
- Bug en `SaleResponse`, el número de ticket se estaba parseando incorrectamente cuando contenía letras.

### Changed

- `SaleResponse (int, int)` se marca como deprecado en favor de `SaleResponse(int, string)`.

## [2.1.1] - 2019-12-19

### Fix

- Bug parsing date and account number

## [2.1.0] - 2019-11-21

### Added

- Soporte para la versión `v3.0.0` de la DLL en C
- `Sale`
  - `ticket` puede ser un `string` ahora.
  - Verifica si `ticket` tiene 6 caracteres (obligatorio)
- Clase `OnepayPayment` para comenzar un pago usando Onepay.

### Fix

- Problema al llamar al connect más de una vez.
- Problema al no seleccionar una opción en el POS al iniciar la venta.

## [2.0.0] - 2019-07-01

### Added

- Método `Sales Detail` para obtener el detalle de ventas.

### Changed

- `LoadKeys`
  - `Terminal ID` tipo de dato de `int` a `string`.
- `Refund`
  - `Terminal ID` tipo de dato de `int` a `string`.
  - `Authorization Code` tipo de dato de `int` a `string`.

## [1.4.0] - 2019-06-18

### Added

- Función para Anulación.
- Función para Última Venta.
- Función para Totales.

## [1.0.0] - 2019-05-13

### Added

- Función para listar puertos seriales.
- Función para POLL de POS.
- Función para Cambio a POS Normal.
- Función para Carga de Llaves.
- Función para Cierre.
- Función para Venta sin mensajes intermedios.
