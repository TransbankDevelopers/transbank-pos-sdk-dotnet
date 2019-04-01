using System.Collections.Generic;

namespace Transbank.POS.Utils
{
    public static class ResponseCodes
    {
        public static readonly Dictionary<int, string> Map = new Dictionary<int, string>
        {
            {00, "Aprobado"},
            {01, "Rechazado"},
            {02, "Host no Responde"},
            {03, "Conexión Fallo"},
            {04, "Transacción ya Fue Anulada"},
            {05, "No existe Transacción para Anular"},
            {06, "Tarjeta no Soportada"},
            {07, "Transacción Cancelada desde el POS"},
            {08, "No puede Anular Transacción Debito"},
            {09, "Error Lectura Tarjeta"},
            {10, "Monto menor al mínimo permitido"},
            {11, "No existe venta"},
            {12, "Transacción No Soportada"},
            {13, "Debe ejecutar cierre "},
            {14, "No hay Tono"},
            {15, "Archivo BITMAP.DAT no encontrado. Favor cargue"},
            {16, "Error Formato Respuesta del HOST"},
            {17, "Error en los 4 últimos dígitos."},
            {18, "Menú invalido"},
            {19, "ERROR_TARJ_DIST"},
            {20, "Tarjeta Invalida"},
            {21, "Anulación. No Permitida"},
            {22, "TIMEOUT"},
            {24, "Impresora Sin Papel"},
            {25, "Fecha Invalida"},
            {26, "Debe Cargar Llaves"},
            {27, "Debe Actualizar"},
            {60, "Error en Número de Cuotas"},
            {61, "Error en Armado de Solicitud"},
            {62, "Problema con el Pinpad interno"},
            {65, "Error al Procesar la Respuesta del Host"},
            {67, "Superó Número Máximo de Ventas, Debe Ejecutar Cierre"},
            {68, "Error Genérico, Falla al Ingresar Montos"},
            {70, "Error de formato Campo de Boleta MAX 6"},
            {71, "Error de Largo Campo de Impresión"},
            {72, "Error de Monto Venta, Debe ser Mayor que 0"},
            {73, "Terminal ID no configurado"},
            {74, "Debe Ejecutar CIERRE"},
            {75, "Comercio no tiene Tarjetas Configuradas"},
            {76, "Supero Número Máximo de Ventas, Debe Ejecutar CIERRE"},
            {77, "Debe Ejecutar Cierre"},
            {78, "Esperando Leer Tarjeta"},
            {79, "Solicitando Confirmar Monto"},
            {81, "Solicitando Ingreso de Clave"},
            {82, "Enviando transacción al Host"},
            {88, "Error Cantidad Cuotas"},
            {93, "Declinada"},
            {94, "Error al Procesar Respuesta"},
            {95, "Error al Imprimir TASA"}
        };
    }
}
