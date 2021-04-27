# Changelog

Todos los cambios notables a este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
y este proyecto adhiere a [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2021-04-27

Se añade soporte para POS Autoservicio.

### Added

- Soporte para equipo POS Autoservicio y sus siguientes métodos, respuestas y excepciones:
- Soporte para poll en el método `Poll`.
- Soporte para carga de llaves en el método `LoadKeys`.
- Soporte para devolución en el método `Refund`.
- Soporte para inicialización en el método `Initialization`.
- Soporte para respuesta de inicialización en el método `InitializationResponse`.
  - Nuevo objeto de respuesta `InitializationResponse`.
  - Nueva excepción `TransbankInitializationResponseException`.
- Soporte para venta en el método `Sale`.
  - Nuevo objeto de respuesta `SaleResponse`.
  - Nueva excepción `TransbankSaleException`.
- Soporte para venta multicódigo en el método `MultiCodeSale`.
  - Nuevo objeto de respuesta `MultiCodeSaleResponse`.
  - Nueva excepción `TransbankMultiCodeSaleException`.
- Soporte para última venta en el método `LastSale`.
  - Nuevo objeto de respuesta `LastSaleResponse`.
  - Nueva excepción `TransbankLastSaleException`.
- Soporte para venta en el método `Close`.
  - Nuevo objeto de respuesta `CloseResponse`.
  - Nueva excepción `TransbankCloseException`.

### Changed

- Se cambia la clase POS por las clases POSIntegrado y POSAutoservicio.
- Los métodos ahora son asíncronos.
- Las respuestas y excepciones están clasificadas por comunes, de POS integrado y POS autoservicio.

## [2.3.1] - 2021-03-24

Se soluciona problema que producía que la aplicación dejara de responder al momento de realizar una operación.

### Fix

- Bug al realizar la lectura del puerto serie, provocando que la aplicación dejara de responder.

## [2.3.0] - 2020-11-19

Se elimina el uso de las antiguas librerías en C, en favor de una librería nativa de C# que mantiene la compatibilidad multiplataforma.

### Added

- Soporte para nuevos equipos Ingenico Desk3500.
- Soporte para venta multicódigo en el método `MultiCodeSale`.
  - Nuevo objeto de respuesta `MultiCodeDetails`.
  - Nueva excepción `TransbankMultiCodeSaleException`.
- Soporte para rescatar la última venta multicódigo en el método `MultiCodeLastSale`.
  - Nuevo objeto de respuesta `MultiCodeLastSaleResponse`.
  - Nueva excepción `TransbankMultiCodeLastSaleException`.
- Soporte para rescatar el detalle de ventas multicódigo en el método `MultiCodeDetails`.
  - Nuevo objeto de respuesta `MultiCodeDetailResponse`.
  - Nueva excepción `TransbankMultiCodeDetailException`.

### Changed

- Se elimina la dependencia del Wrapper en C y de Libserialport, reemplazándola por el uso de la librería Nuget `System.IO.Ports`.

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
- Función para Poll de POS.
- Función para Cambio a POS Normal.
- Función para Carga de Llaves.
- Función para Cierre.
- Función para Venta sin mensajes intermedios.
