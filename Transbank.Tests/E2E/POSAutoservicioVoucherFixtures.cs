namespace Transbank.Tests.E2E
{
    internal static class POSAutoservicioVoucherFixtures
    {
        internal const string SaleDebitVoucher =
            "          COMPROBANTE DE VENTA          " +
            "           TARJETA DE DEBITO            " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "          597029414300-M261L1           " +
            "FECHA             HORA          TERMINAL" +
            "18/03/26        12:32:30        IM750164" +
            "FECHA CONTABLE                  00-00-00" +
            "NUMERO DE TARJETA   NUM DE CUENTA  MARCA" +
            "************3331      ********331   B-DB" +
            "OTRA                                    " +
            "TOTAL:                           $ 1.000" +
            "NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000055" +
            "CODIGO DE AUTORIZACION:           547545" +
            "                                        " +
            "                                        " +
            "         GRACIAS POR SU COMPRA          " +
            " ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";

        internal const string SaleCreditVoucher =
            "          COMPROBANTE DE VENTA          " +
            "             PAGO EN CUOTAS             " +
            "           TARJETA DE CREDITO           " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "          597029414300-M261L1           " +
            "FECHA             HORA          TERMINAL" +
            "18/03/26        12:34:29        IM750164" +
            "                                        " +
            "NUMERO DE TARJETA                   B-CR" +
            "************6590                        " +
            "VISA                                    " +
            "TOTAL:                          $ 10.000" +
            "NUMERO DE CUOTAS:                     03" +
            "TIPO DE CUOTAS:       CUOTAS SIN INTERES" +
            "MONTO CUOTA:                     $ 3.334" +
            "TASA DE INTERES:                  00.00%" +
            "NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000057" +
            "CODIGO DE AUTORIZACION:           316557" +
            "                                        " +
            "         GRACIAS POR SU COMPRA          " +
            " ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";

        internal const string MultiCodeSaleDebitVoucher =
            "          COMPROBANTE DE VENTA          " +
            "           TARJETA DE DEBITO            " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "          597029414303-M261M1           " +
            "FECHA             HORA          TERMINAL" +
            "18/03/26        17:10:40        IM750164" +
            "FECHA CONTABLE                  00-00-00" +
            "NUMERO DE TARJETA   NUM DE CUENTA  MARCA" +
            "************3331      ********331   B-DB" +
            "OTRA                                    " +
            "TOTAL:                           $ 1.000" +
            "NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000062" +
            "CODIGO DE AUTORIZACION:           475618" +
            "                                        " +
            "                                        " +
            "         GRACIAS POR SU COMPRA          " +
            " ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";

        internal const string MultiCodeSaleCreditVoucher =
            "          COMPROBANTE DE VENTA          " +
            "             PAGO EN CUOTAS             " +
            "           TARJETA DE CREDITO           " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "          597029414303-M261M1           " +
            "FECHA             HORA          TERMINAL" +
            "18/03/26        17:11:53        IM750164" +
            "                                        " +
            "NUMERO DE TARJETA                   B-CR" +
            "************6590                        " +
            "VISA                                    " +
            "TOTAL:                          $ 10.000" +
            "NUMERO DE CUOTAS:                     03" +
            "TIPO DE CUOTAS:       CUOTAS SIN INTERES" +
            "MONTO CUOTA:                     $ 3.334" +
            "TASA DE INTERES:                  00.00%" +
            "NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000064" +
            "CODIGO DE AUTORIZACION:           194937" +
            "                                        " +
            "         GRACIAS POR SU COMPRA          " +
            " ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";

        internal const string LastSaleDebitVoucher =
            "          COMPROBANTE DE VENTA          " +
            "           TARJETA DE DEBITO            " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "           *** DUPLICADO ***            " +
            "          597029414303-M261M1           " +
            "FECHA             HORA          TERMINAL" +
            "19/03/26        10:24:38        IM750164" +
            "FECHA CONTABLE                  00-00-00" +
            "NUMERO DE TARJETA   NUM DE CUENTA  MARCA" +
            "************3331      ********331   B-DB" +
            "OTRA                                    " +
            "TOTAL:                           $ 1.000" +
            "NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000068" +
            "CODIGO DE AUTORIZACION:           912108" +
            "                                        " +
            "                                        " +
            "         GRACIAS POR SU COMPRA          " +
            " ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";

        internal const string LastSaleCreditVoucher =
            "          COMPROBANTE DE VENTA          " +
            "             PAGO EN CUOTAS             " +
            "           TARJETA DE CREDITO           " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "           *** DUPLICADO ***            " +
            "          597029414300-M261L1           " +
            "FECHA             HORA          TERMINAL" +
            "17/03/26        11:50:06        IM750164" +
            "                                        " +
            "NUMERO DE TARJETA                   B-CR" +
            "************6590                        " +
            "VISA                                    " +
            "TOTAL:                          $ 10.000" +
            "NUMERO DE CUOTAS:                     03" +
            "TIPO DE CUOTAS:       CUOTAS SIN INTERES" +
            "MONTO CUOTA:                     $ 3.334" +
            "TASA DE INTERES:                  00.00%" +
            "NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000034" +
            "CODIGO DE AUTORIZACION:           575354" +
            "                                        " +
            "         GRACIAS POR SU COMPRA          " +
            " ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";

        internal const string CloseWithDataVoucher =
            "    REPORTE DEL CIERRE DEL TERMINAL     " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "          597029414300-M261L1           " +
            "FECHA             HORA          TERMINAL" +
            "17/03/26        12:06:39        IM750164" +
            "                                        " +
            "               NUMERO              TOTAL" +
            "VISA             002             $20.000" +
            "----------------------------------------" +
            "TOTAL CAPTURAS   002             $20.000";

        internal const string CloseWithoutDataVoucher =
            "    REPORTE DEL CIERRE DEL TERMINAL     " +
            "                  Tbk                   " +
            "                  MATI                  " +
            "                Santiago                " +
            "               11111111-1               " +
            "                SANTIAGO                " +
            "          597029414300-M261L1           " +
            "FECHA             HORA          TERMINAL" +
            "17/03/26        12:08:10        IM750164" +
            "                                        " +
            "               NUMERO              TOTAL" +
            "----------------------------------------" +
            "TOTAL CAPTURAS   000                  $0";
    }
}
