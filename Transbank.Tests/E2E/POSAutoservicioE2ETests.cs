using System.Linq;
using System.Threading.Tasks;
using System;
using Transbank.Responses.CommonResponses;
using Transbank.Responses.AutoservicioResponse;
using Transbank.Services;
using Transbank.Tests.Mocks;
using Xunit;

namespace Transbank.Tests.E2E
{
    public class POSAutoservicioE2ETests
    {
        private readonly MockSerialHandler _mockHandler;
        private readonly POSAutoservicio.POSAutoservicio _pos;
        private const char STX = (char)0x02;
        private const char ETX = (char)0x03;
        private const char ACK = (char)0x06;
        private const string SaleDebitWithVoucherResponsePayload =
            "0210|00|597029414300|IM750164|123456|547545|1000|3331|55|DB|00-00-00|331|P |18032026|123230|" +
            "          COMPROBANTE DE VENTA                     TARJETA DE DEBITO                              Tbk" +
            "                                     MATI                                  Santiago                " +
            "               11111111-1                               SANTIAGO                          597029414300-M261L1" +
            "           FECHA             HORA          TERMINAL18/03/26        12:32:30        IM750164FECHA CONTABLE" +
            "                  00-00-00NUMERO DE TARJETA   NUM DE CUENTA  MARCA************3331      ********331   B-DB" +
            "OTRA                                    TOTAL:                           $ 1.000NUMERO DE BOLETA:                 123456" +
            "NUMERO DE OPERACION:              000055CODIGO DE AUTORIZACION:           547545" +
            "                                                                                         GRACIAS POR SU COMPRA" +
            "           ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";
        private const string SaleCreditWithVoucherResponsePayload =
            "0210|00|597029414300|IM750164|123456|316557|10000|6590|57|CR|||VI|18032026|123429|" +
            "          COMPROBANTE DE VENTA                       PAGO EN CUOTAS                        TARJETA DE CREDITO" +
            "                             Tbk                                     MATI                                  Santiago" +
            "                               11111111-1                               SANTIAGO                          597029414300-M261L1" +
            "           FECHA             HORA          TERMINAL18/03/26        12:34:29        IM750164" +
            "                                        NUMERO DE TARJETA                   B-CR************6590" +
            "                        VISA                                    TOTAL:                          $ 10.000NUMERO DE CUOTAS:" +
            "                     03TIPO DE CUOTAS:       CUOTAS SIN INTERESMONTO CUOTA:                     $ 3.334TASA DE INTERES:" +
            "                  00.00%NUMERO DE BOLETA:                 123456NUMERO DE OPERACION:              000057CODIGO DE AUTORIZACION:" +
            "           316557                                                 GRACIAS POR SU COMPRA           ACEPTO PAGAR SEGUN CONTRATO CON EMISOR |03|03|3334|CUOTAS SIN INTERES";
        private const string LastSaleDebitWithVoucherResponsePayload =
            "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|10032026|331|P |12032026|171142|" +
            "          COMPROBANTE DE VENTA                     TARJETA DE DEBITO                              Tbk" +
            "                                     MATI                                  Santiago                " +
            "               11111111-1                               SANTIAGO                           *** DUPLIC" +
            "ADO ***                      597029414300-M261L1           FECHA             HORA          TERMINAL" +
            "12/03/26        17:11:42        IM750164FECHA CONTABLE                  10032026NUMERO DE TARJETA " +
            "  NUM DE CUENTA  MARCA************3331      ********331   B-DBOTRA                                    " +
            "TOTAL:                           $ 1.000NUMERO DE BOLETA:                 123456NUMERO DE OPERACION" +
            ":              000056CODIGO DE AUTORIZACION:           574062                              " +
            "                                           GRACIAS POR SU COMPRA           ACEPTO PAGAR SEGUN CONTRATO CON EMISOR ";
        private const string LastSaleCreditWithVoucherResponsePayload =
            "0260|00|597029414300|IM750164|123456|575354|10000|6590|34|CR|||VI|17032026|115006|" +
            "          COMPROBANTE DE VENTA                       PAGO EN CUOTAS                        TARJETA DE CREDITO" +
            "                             Tbk                                     MATI                                  Santiago" +
            "                               11111111-1                               SANTIAGO                           *** DUPLICADO ***" +
            "                      597029414300-M261L1           FECHA             HORA          TERMINAL17/03/26        11:50:06" +
            "        IM750164                                        NUMERO DE TARJETA                   B-CR************6590" +
            "                        VISA                                    TOTAL:                          $ 10.000NUMERO DE CUOTAS:" +
            "                     03TIPO DE CUOTAS:       CUOTAS SIN INTERESMONTO CUOTA:                     $ 3.334TASA DE INTERES:" +
            "                  00.00%NUMERO DE BOLETA:                 123456NUMERO DE OPERACION:              000034CODIGO DE AUTORIZACION:" +
            "           575354                                                 GRACIAS POR SU COMPRA           ACEPTO PAGAR SEGUN CONTRATO CON EMISOR |03|03|3334|CUOTAS SIN INTERES";
        private const string CloseWithDataVoucherResponsePayload =
            "0510|00|597029414300|IM750164|" +
            "    REPORTE DEL CIERRE DEL TERMINAL                       Tbk                                     MATI                                  Santiago                " +
            "               11111111-1                               SANTIAGO                          597029414300-M261L1           FECHA             HORA          TERMINAL" +
            "17/03/26        12:06:39        IM750164                                                       NUMERO              TOTAL" +
            "VISA             002             $20.000----------------------------------------TOTAL CAPTURAS   002             $20.000";
        private const string CloseWithoutDataVoucherResponsePayload =
            "0510|00|597029414300|IM750164|" +
            "    REPORTE DEL CIERRE DEL TERMINAL                       Tbk                                     MATI                                  Santiago                " +
            "               11111111-1                               SANTIAGO                          597029414300-M261L1           FECHA             HORA          TERMINAL" +
            "17/03/26        12:08:10        IM750164                                                       NUMERO              TOTAL" +
            "----------------------------------------TOTAL CAPTURAS   000                  $0";

        public POSAutoservicioE2ETests()
        {
            _mockHandler = new MockSerialHandler();
            var service = new PosService(_mockHandler, PosService.Model.AUTOSERVICIO);
            _pos = new POSAutoservicio.POSAutoservicio(_mockHandler, service);
        }

        [Fact]
        public async Task Poll_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0100";

            var task = _pos.Poll();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }

        [Fact]
        public async Task Initialization_ShouldSendExpectedCommand_AndCompleteOnAck()
        {
            const string expectedCommandPayload = "0070";

            var task = _pos.Initialization();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());

            bool response = await task;

            Assert.True(response);
            Assert.Single(_mockHandler.WrittenData);
        }

        [Fact]
        public async Task InitializationResponse_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0080";
            const string responsePayload = "1080|90|03022026|111543";

            var task = _pos.InitializationResponse();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            InitializationResponse response = await task;
            string initializationResponseText = response.ToString();

            AssertBasicResponse(response, "1080", 90, success: true, 0, null);
            Assert.True(response.Success);
            Assert.Equal(new DateTime(2026, 2, 3, 11, 15, 43), response.RealDate);
            AssertBaseResponseText(initializationResponseText, "1080", 90);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LoadKeys_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            AssertBasicResponse(response, "0810", 0, success: true, 597029414300, "IM750164");
            AssertBaseResponseText(loadKeysResponseText, "0810", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task ShouldSendNackAndKeepWaiting_WhenResponseLrcIsInvalid()
        {
            const string commandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            AssertSentCommand(commandPayload);
            SendAck();
            _mockHandler.SimulateIncoming(BuildFrameWithInvalidLrc(responsePayload));

            Assert.False(task.IsCompleted);
            Assert.Equal(((char)0x15).ToString(), _mockHandler.WrittenData.Last());

            SendResponse(responsePayload);

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            AssertBasicResponse(response, "0810", 0, success: true, 597029414300, "IM750164");
            AssertBaseResponseText(loadKeysResponseText, "0810", 0);
            AssertFinalAckWritten(3);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedDebitResponseWithVoucher()
        {
            const string expectedCommandPayload = "0200|1000|123456|1|0";
            var task = _pos.Sale(1000, "123456", sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(SaleDebitWithVoucherResponsePayload);

            SaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "547545", 1000, 3331, 55, "DB", "331", "P", new DateTime(2026, 3, 18, 12, 32, 30));
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, -1, -1, -1, string.Empty);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedDebitResponseWithoutVoucher()
        {
            const string expectedCommandPayload = "0200|1000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|700527|1000|3331|56|DB|00-00-00|331|P |18032026|123307";

            var task = _pos.Sale(1000, "123456");

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "700527", 1000, 3331, 56, "DB", "331", "P", new DateTime(2026, 3, 18, 12, 33, 7));
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, -1, -1, -1, string.Empty);
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0200|10000|123456|1|0";
            var task = _pos.Sale(10000, "123456", sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendFragmentedResponseAndAssertPending(task, SaleCreditWithVoucherResponsePayload);

            SaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "316557", 10000, 6590, 57, "CR", string.Empty, "VI", new DateTime(2026, 3, 18, 12, 34, 29));
            Assert.Null(response.AccountingDate);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("PAGO EN CUOTAS", voucher);
            Assert.Contains("TARJETA DE CREDITO", voucher);
            Assert.Contains("NUMERO DE CUOTAS", voucher);
            Assert.Contains("TIPO DE CUOTAS", voucher);
            Assert.Contains("CUOTAS SIN INTERES", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedCreditResponseWithoutVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0200|10000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|776549|10000|6590|58|CR|||VI|18032026|123506||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.Sale(10000, "123456");

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(responsePayload);

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            AssertBasicResponse(response, "0210", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "776549", 10000, 6590, 58, "CR", string.Empty, "VI", new DateTime(2026, 3, 18, 12, 35, 6));
            Assert.Null(response.AccountingDate);
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(saleResponseText, "0210", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedDebitResponseWithoutVoucher_WhenThereIsALastSale()
        {
            const string expectedCommandPayload = "0250|0";
            const string responsePayload = "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|10032026|331|P |12032026|171142";

            var task = _pos.LastSale();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;
            string lastSaleResponseText = response.ToString();

            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("574062", response.AuthorizationCode);
            Assert.Equal(1000, response.Amount);
            Assert.Equal(3331, response.Last4Digits);
            Assert.Equal(56, response.OperationNumber);
            Assert.Equal("DB", response.CardType);
            Assert.Equal(new DateTime(2026, 3, 10), response.AccountingDate);
            Assert.Equal("331", response.AccountNumber);
            Assert.Equal("P", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 12, 17, 11, 42), response.RealDate);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            Assert.Equal(-1, response.SharesType);
            Assert.Equal(-1, response.SharesNumber);
            Assert.Equal(-1, response.SharesAmount);
            Assert.Equal(string.Empty, response.SharesTypeGloss);
            Assert.Contains("Function: 0260", lastSaleResponseText);
            Assert.Contains("Response code:0", lastSaleResponseText);
            Assert.Contains("Card Type: DB", lastSaleResponseText);
            Assert.Contains("Card Brand: P", lastSaleResponseText);
            Assert.Contains("Shares Type: -1", lastSaleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task LastSale_ShouldParseAccountingDate_WhenDateIsValid()
        {
            const string responsePayload = "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|10032026|331|P |12032026|171142";

            var task = _pos.LastSale();

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;

            Assert.Equal(new DateTime(2026, 3, 10), response.AccountingDate);
        }

        [Fact]
        public async Task LastSale_ShouldReturnMinValueAccountingDate_WhenDateIsInvalid()
        {
            const string responsePayload = "0260|00|597029414300|IM750164|123456|574062|1000|3331|56|DB|abc|331|P |12032026|171142";

            var task = _pos.LastSale();

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;

            Assert.Equal(DateTime.MinValue, response.AccountingDate);
        }

        [Fact]
        public async Task LastSale_ShouldReturnNullAccountingDate_WhenDateIsMissing()
        {
            const string responsePayload = "0260|00|597029414300|IM750164|123456|575354|10000|6590|34|CR|||VI|17032026|115006||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.LastSale();

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;

            Assert.Null(response.AccountingDate);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedDebitResponseWithVoucher_WhenVoucherIsRequested()
        {
            const string expectedCommandPayload = "0250|1";
            var task = _pos.LastSale(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(LastSaleDebitWithVoucherResponsePayload);

            LastSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string lastSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0260", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "574062", 1000, 3331, 56, "DB", "331", "P", new DateTime(2026, 3, 12, 17, 11, 42));
            Assert.Equal(new DateTime(2026, 3, 10), response.AccountingDate);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, -1, -1, -1, string.Empty);
            AssertBaseResponseText(lastSaleResponseText, "0260", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0250|1";
            var task = _pos.LastSale(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendFragmentedResponseAndAssertPending(task, LastSaleCreditWithVoucherResponsePayload);

            LastSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string lastSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0260", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "575354", 10000, 6590, 34, "CR", string.Empty, "VI", new DateTime(2026, 3, 17, 11, 50, 6));
            Assert.Null(response.AccountingDate);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("PAGO EN CUOTAS", voucher);
            Assert.Contains("TARJETA DE CREDITO", voucher);
            Assert.Contains("NUMERO DE CUOTAS", voucher);
            Assert.Contains("TIPO DE CUOTAS", voucher);
            Assert.Contains("CUOTAS SIN INTERES", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(lastSaleResponseText, "0260", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedCreditResponseWithoutVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0250|0";
            const string responsePayload = "0260|00|597029414300|IM750164|123456|575354|10000|6590|34|CR|||VI|17032026|115006||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.LastSale();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;
            string lastSaleResponseText = response.ToString();

            AssertBasicResponse(response, "0260", 0, success: true, 597029414300, "IM750164");
            AssertSaleFields(response, "123456", "575354", 10000, 6590, 34, "CR", string.Empty, "VI", new DateTime(2026, 3, 17, 11, 50, 6));
            Assert.Null(response.AccountingDate);
            AssertEmptyPrintingField(response.PrintingField);
            AssertInstallments(response, 3, 3, 3334, "CUOTAS SIN INTERES");
            AssertBaseResponseText(lastSaleResponseText, "0260", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task LastSale_ShouldParseNoSaleResponse_WhenThereIsNoLastSale()
        {
            const string expectedCommandPayload = "0250|0";
            const string responsePayload = "0260|11|597029414300|IM750164";

            var task = _pos.LastSale();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            var response = await task;
            string lastSaleResponseText = response.ToString();

            Assert.NotNull(response);
            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(11, response.ResponseCode);
            Assert.False(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal(string.Empty, response.Ticket);
            Assert.Equal(string.Empty, response.AuthorizationCode);
            Assert.Equal(-1, response.Amount);
            Assert.Equal(-1, response.Last4Digits);
            Assert.Equal(-1, response.OperationNumber);
            Assert.Equal(string.Empty, response.CardType);
            Assert.Equal(string.Empty, response.CardBrand);
            Assert.Null(response.AccountingDate);
            Assert.Null(response.RealDate);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            Assert.Equal(-1, response.SharesType);
            Assert.Equal(-1, response.SharesNumber);
            Assert.Equal(-1, response.SharesAmount);
            Assert.Equal(string.Empty, response.SharesTypeGloss);
            AssertBaseResponseText(lastSaleResponseText, "0260", 11);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithVoucher_WhenThereAreCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|1";
            var task = _pos.Close(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(CloseWithDataVoucherResponsePayload);

            CloseResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string closeResponseText = response.ToString();

            AssertBasicResponse(response, "0510", 0, success: true, 597029414300, "IM750164");
            Assert.Contains("REPORTE DEL CIERRE DEL TERMINAL", voucher);
            Assert.Contains("NUMERO              TOTAL", voucher);
            Assert.Contains("VISA", voucher);
            Assert.Contains("TOTAL CAPTURAS", voucher);
            Assert.Contains("$20.000", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithoutVoucher_WhenThereAreCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|0";
            const string responsePayload = "0510|00|597029414300|IM750164|";

            var task = _pos.Close(sendVoucher: false);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            Assert.Equal("0510", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithVoucher_WhenThereAreNoCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|1";
            var task = _pos.Close(sendVoucher: true);

            AssertSentCommand(expectedCommandPayload);
            SendAck();
            SendResponse(CloseWithoutDataVoucherResponsePayload);

            CloseResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string closeResponseText = response.ToString();

            AssertBasicResponse(response, "0510", 0, success: true, 597029414300, "IM750164");
            Assert.Contains("REPORTE DEL CIERRE DEL TERMINAL", voucher);
            Assert.Contains("NUMERO              TOTAL", voucher);
            Assert.DoesNotContain("VISA", voucher);
            Assert.Contains("TOTAL CAPTURAS", voucher);
            Assert.Contains("$0", voucher);
            AssertVoucherLinesHaveFixedWidth(response.PrintingField);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithoutVoucher_WhenThereAreNoCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|0";
            const string responsePayload = "0510|00|597029414300|IM750164|";

            var task = _pos.Close(sendVoucher: false);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            CloseResponse response = await task;
            string closeResponseText = response.ToString();

            Assert.Equal("0510", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            AssertBaseResponseText(closeResponseText, "0510", 0);
            AssertFinalAckWritten(2);
        }

        private static string BuildCommandFrame(string payload)
        {
            char lrc = ETX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            return $"{STX}{payload}{ETX}{lrc}";
        }

        private static string BuildResponseFrame(string payload)
        {
            char lrc = STX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            lrc ^= ETX;
            return $"{STX}{payload}{ETX}{lrc}";
        }

        private static string BuildFrameWithInvalidLrc(string payload)
        {
            return $"{STX}{payload}{ETX}{(char)0x00}";
        }

        private void AssertSentCommand(string payload)
        {
            Assert.Equal(BuildCommandFrame(payload), _mockHandler.WrittenData.Single());
        }

        private void SendAck()
        {
            _mockHandler.SimulateIncoming(ACK.ToString());
        }

        private void SendResponse(string payload)
        {
            _mockHandler.SimulateIncoming(BuildResponseFrame(payload));
        }

        private void SendFragmentedResponseAndAssertPending(Task task, string payload)
        {
            string fullResponseFrame = BuildResponseFrame(payload);
            int splitIndex = fullResponseFrame.Length / 2;
            _mockHandler.SimulateIncoming(fullResponseFrame[..splitIndex]);
            Assert.False(task.IsCompleted);
            _mockHandler.SimulateIncoming(fullResponseFrame[splitIndex..]);
        }

        private void AssertFinalAckWritten(int expectedWriteCount)
        {
            Assert.Equal(expectedWriteCount, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        private static void AssertBasicResponse(BasicResponse response, string functionCode, int responseCode, bool success, long commerceCode, string? terminalId)
        {
            Assert.NotNull(response);
            Assert.Equal(functionCode, response.FunctionCode);
            Assert.Equal(responseCode, response.ResponseCode);
            Assert.Equal(success, response.Success);

            if (commerceCode != 0 && response is LoadKeysResponse loadKeysResponse)
            {
                Assert.Equal(commerceCode, loadKeysResponse.CommerceCode);
            }

            if (terminalId != null && response is LoadKeysResponse loadKeysTerminalResponse)
            {
                Assert.Equal(terminalId, loadKeysTerminalResponse.TerminalId);
            }
        }

        private static void AssertBaseResponseText(string responseText, string functionCode, int responseCode)
        {
            Assert.Contains($"Function: {functionCode}", responseText);
            Assert.Contains($"Response code:{responseCode}", responseText);
        }

        private static void AssertSaleFields(SaleResponse response, string ticket, string authorizationCode, int amount, int last4Digits, int operationNumber, string cardType, string accountNumber, string cardBrand, DateTime realDate)
        {
            Assert.Equal(ticket, response.Ticket);
            Assert.Equal(authorizationCode, response.AuthorizationCode);
            Assert.Equal(amount, response.Amount);
            Assert.Equal(last4Digits, response.Last4Digits);
            Assert.Equal(operationNumber, response.OperationNumber);
            Assert.Equal(cardType, response.CardType);
            Assert.Equal(accountNumber, response.AccountNumber);
            Assert.Equal(cardBrand, response.CardBrand);
            Assert.Equal(realDate, response.RealDate);
        }

        private static void AssertInstallments(SaleResponse response, int sharesType, int sharesNumber, int sharesAmount, string sharesTypeGloss)
        {
            Assert.Equal(sharesType, response.SharesType);
            Assert.Equal(sharesNumber, response.SharesNumber);
            Assert.Equal(sharesAmount, response.SharesAmount);
            Assert.Equal(sharesTypeGloss, response.SharesTypeGloss);
        }

        private static void AssertEmptyPrintingField(System.Collections.Generic.IReadOnlyList<string> printingField)
        {
            Assert.Single(printingField);
            Assert.Equal(string.Empty, printingField[0]);
        }

        private static void AssertVoucherLinesHaveFixedWidth(System.Collections.Generic.IReadOnlyList<string> printingField)
        {
            Assert.NotEmpty(printingField);
            if (printingField.Count > 1)
            {
                Assert.All(printingField, line => Assert.Equal(40, line.Length));
            }
        }
    }
}
