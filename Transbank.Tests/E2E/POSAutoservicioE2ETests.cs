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

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            InitializationResponse response = await task;
            string initializationResponseText = response.ToString();

            Assert.NotNull(response);
            Assert.Equal("1080", response.FunctionCode);
            Assert.Equal(90, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(new DateTime(2026, 2, 3, 11, 15, 43), response.RealDate);
            Assert.Contains("Function: 1080", initializationResponseText);
            Assert.Contains("Response code:90", initializationResponseText);
            Assert.Contains("Response message: Inicialización Exitosa", initializationResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task LoadKeys_ShouldSendExpectedCommand_AndParseApprovedResponse()
        {
            const string expectedCommandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            Assert.NotNull(response);
            Assert.Equal("0810", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Contains("Function: 0810", loadKeysResponseText);
            Assert.Contains("Response code:0", loadKeysResponseText);
            Assert.Contains("Commerce Code: 597029414300", loadKeysResponseText);
            Assert.Contains("Terminal Id: IM750164", loadKeysResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task ShouldSendNackAndKeepWaiting_WhenResponseLrcIsInvalid()
        {
            const string commandPayload = "0800";
            const string responsePayload = "0810|00|597029414300|IM750164";

            var task = _pos.LoadKeys();

            Assert.Equal(BuildCommandFrame(commandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildFrameWithInvalidLrc(responsePayload));

            Assert.False(task.IsCompleted);
            Assert.Equal(((char)0x15).ToString(), _mockHandler.WrittenData.Last());

            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LoadKeysResponse response = await task;
            string loadKeysResponseText = response.ToString();

            Assert.Equal("0810", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Contains("Function: 0810", loadKeysResponseText);
            Assert.Contains("Response code:0", loadKeysResponseText);
            Assert.Contains("Commerce Code: 597029414300", loadKeysResponseText);
            Assert.Contains("Terminal Id: IM750164", loadKeysResponseText);
            Assert.Equal(3, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedDebitResponseWithVoucher()
        {
            const string expectedCommandPayload = "0200|1000|123456|1|0";
            const string responsePayload =
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

            var task = _pos.Sale(1000, "123456", sendVoucher: true);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            SaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string saleResponseText = response.ToString();

            Assert.Equal("0210", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("547545", response.AuthorizationCode);
            Assert.Equal(1000, response.Amount);
            Assert.Equal(3331, response.Last4Digits);
            Assert.Equal(55, response.OperationNumber);
            Assert.Equal("DB", response.CardType);
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal("331", response.AccountNumber);
            Assert.Equal("P", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 18, 12, 32, 30), response.RealDate);
            Assert.NotEmpty(response.PrintingField);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            Assert.Equal(-1, response.SharesType);
            Assert.Equal(-1, response.SharesNumber);
            Assert.Equal(-1, response.SharesAmount);
            Assert.Equal(string.Empty, response.SharesTypeGloss);
            Assert.Contains("Function: 0210", saleResponseText);
            Assert.Contains("Response code:0", saleResponseText);
            Assert.Contains("Card Type: DB", saleResponseText);
            Assert.Contains("Card Brand: P", saleResponseText);
            Assert.Contains("Printing Field:", saleResponseText);
            Assert.Contains("GRACIAS POR SU COMPRA", saleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedDebitResponseWithoutVoucher()
        {
            const string expectedCommandPayload = "0200|1000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|700527|1000|3331|56|DB|00-00-00|331|P |18032026|123307";

            var task = _pos.Sale(1000, "123456");

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            Assert.Equal("0210", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("700527", response.AuthorizationCode);
            Assert.Equal(1000, response.Amount);
            Assert.Equal(3331, response.Last4Digits);
            Assert.Equal(56, response.OperationNumber);
            Assert.Equal("DB", response.CardType);
            Assert.Equal(DateTime.MinValue, response.AccountingDate);
            Assert.Equal("331", response.AccountNumber);
            Assert.Equal("P", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 18, 12, 33, 7), response.RealDate);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            Assert.Equal(-1, response.SharesType);
            Assert.Equal(-1, response.SharesNumber);
            Assert.Equal(-1, response.SharesAmount);
            Assert.Equal(string.Empty, response.SharesTypeGloss);
            Assert.Contains("Function: 0210", saleResponseText);
            Assert.Contains("Response code:0", saleResponseText);
            Assert.Contains("Card Type: DB", saleResponseText);
            Assert.Contains("Card Brand: P", saleResponseText);
            Assert.Contains("Shares Type: -1", saleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0200|10000|123456|1|0";
            const string responsePayload =
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

            var task = _pos.Sale(10000, "123456", sendVoucher: true);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());

            string fullResponseFrame = BuildResponseFrame(responsePayload);
            int splitIndex = fullResponseFrame.IndexOf("                                GRACIAS", StringComparison.Ordinal);
            _mockHandler.SimulateIncoming(fullResponseFrame[..splitIndex]);
            Assert.False(task.IsCompleted);
            _mockHandler.SimulateIncoming(fullResponseFrame[splitIndex..]);

            SaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string saleResponseText = response.ToString();

            Assert.Equal("0210", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("316557", response.AuthorizationCode);
            Assert.Equal(10000, response.Amount);
            Assert.Equal(6590, response.Last4Digits);
            Assert.Equal(57, response.OperationNumber);
            Assert.Equal("CR", response.CardType);
            Assert.Null(response.AccountingDate);
            Assert.Equal(string.Empty, response.AccountNumber);
            Assert.Equal("VI", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 18, 12, 34, 29), response.RealDate);
            Assert.NotEmpty(response.PrintingField);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("PAGO EN CUOTAS", voucher);
            Assert.Contains("TARJETA DE CREDITO", voucher);
            Assert.Contains("NUMERO DE CUOTAS", voucher);
            Assert.Contains("TIPO DE CUOTAS", voucher);
            Assert.Contains("CUOTAS SIN INTERES", voucher);
            Assert.Equal(3, response.SharesType);
            Assert.Equal(3, response.SharesNumber);
            Assert.Equal(3334, response.SharesAmount);
            Assert.Equal("CUOTAS SIN INTERES", response.SharesTypeGloss);
            Assert.Contains("Function: 0210", saleResponseText);
            Assert.Contains("Response code:0", saleResponseText);
            Assert.Contains("Card Type: CR", saleResponseText);
            Assert.Contains("Card Brand: VI", saleResponseText);
            Assert.Contains("Shares Type: 3", saleResponseText);
            Assert.Contains("Shares Type Gloss: CUOTAS SIN INTERES", saleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task Sale_ShouldParseApprovedCreditResponseWithoutVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0200|10000|123456|0|0";
            const string responsePayload = "0210|00|597029414300|IM750164|123456|776549|10000|6590|58|CR|||VI|18032026|123506||03|03|3334|CUOTAS SIN INTERES";

            var task = _pos.Sale(10000, "123456");

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            SaleResponse response = await task;
            string saleResponseText = response.ToString();

            Assert.Equal("0210", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("776549", response.AuthorizationCode);
            Assert.Equal(10000, response.Amount);
            Assert.Equal(6590, response.Last4Digits);
            Assert.Equal(58, response.OperationNumber);
            Assert.Equal("CR", response.CardType);
            Assert.Null(response.AccountingDate);
            Assert.Equal(string.Empty, response.AccountNumber);
            Assert.Equal("VI", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 18, 12, 35, 6), response.RealDate);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            Assert.Equal(3, response.SharesType);
            Assert.Equal(3, response.SharesNumber);
            Assert.Equal(3334, response.SharesAmount);
            Assert.Equal("CUOTAS SIN INTERES", response.SharesTypeGloss);
            Assert.Contains("Function: 0210", saleResponseText);
            Assert.Contains("Response code:0", saleResponseText);
            Assert.Contains("Card Type: CR", saleResponseText);
            Assert.Contains("Card Brand: VI", saleResponseText);
            Assert.Contains("Shares Type: 3", saleResponseText);
            Assert.Contains("Shares Type Gloss: CUOTAS SIN INTERES", saleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
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
            const string responsePayload =
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

            var task = _pos.LastSale(sendVoucher: true);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            LastSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
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
            Assert.NotEmpty(response.PrintingField);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("TARJETA DE DEBITO", voucher);
            Assert.Contains("TOTAL:", voucher);
            Assert.Contains("CODIGO DE AUTORIZACION:", voucher);
            Assert.Contains("GRACIAS POR SU COMPRA", voucher);
            Assert.Equal(-1, response.SharesType);
            Assert.Equal(-1, response.SharesNumber);
            Assert.Equal(-1, response.SharesAmount);
            Assert.Equal(string.Empty, response.SharesTypeGloss);
            Assert.Contains("Function: 0260", lastSaleResponseText);
            Assert.Contains("Response code:0", lastSaleResponseText);
            Assert.Contains("Card Type: DB", lastSaleResponseText);
            Assert.Contains("Card Brand: P", lastSaleResponseText);
            Assert.Contains("Printing Field:", lastSaleResponseText);
            Assert.Contains("GRACIAS POR SU COMPRA", lastSaleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task LastSale_ShouldParseApprovedCreditResponseWithVoucher_WhenAccountingDateIsEmpty()
        {
            const string expectedCommandPayload = "0250|1";
            const string responsePayload =
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

            var task = _pos.LastSale(sendVoucher: true);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());

            string fullResponseFrame = BuildResponseFrame(responsePayload);
            int splitIndex = fullResponseFrame.IndexOf("ACION:", StringComparison.Ordinal);
            _mockHandler.SimulateIncoming(fullResponseFrame[..splitIndex]);
            Assert.False(task.IsCompleted);
            _mockHandler.SimulateIncoming(fullResponseFrame[splitIndex..]);

            LastSaleResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string lastSaleResponseText = response.ToString();

            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("575354", response.AuthorizationCode);
            Assert.Equal(10000, response.Amount);
            Assert.Equal(6590, response.Last4Digits);
            Assert.Equal(34, response.OperationNumber);
            Assert.Equal("CR", response.CardType);
            Assert.Null(response.AccountingDate);
            Assert.Equal(string.Empty, response.AccountNumber);
            Assert.Equal("VI", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 17, 11, 50, 6), response.RealDate);
            Assert.NotEmpty(response.PrintingField);
            Assert.Contains("COMPROBANTE DE VENTA", voucher);
            Assert.Contains("PAGO EN CUOTAS", voucher);
            Assert.Contains("TARJETA DE CREDITO", voucher);
            Assert.Contains("NUMERO DE CUOTAS", voucher);
            Assert.Contains("TIPO DE CUOTAS", voucher);
            Assert.Contains("CUOTAS SIN INTERES", voucher);
            Assert.Equal(3, response.SharesType);
            Assert.Equal(3, response.SharesNumber);
            Assert.Equal(3334, response.SharesAmount);
            Assert.Equal("CUOTAS SIN INTERES", response.SharesTypeGloss);
            Assert.Contains("Function: 0260", lastSaleResponseText);
            Assert.Contains("Response code:0", lastSaleResponseText);
            Assert.Contains("Card Type: CR", lastSaleResponseText);
            Assert.Contains("Card Brand: VI", lastSaleResponseText);
            Assert.Contains("Shares Type: 3", lastSaleResponseText);
            Assert.Contains("Shares Type Gloss: CUOTAS SIN INTERES", lastSaleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
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

            Assert.Equal("0260", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.Equal("123456", response.Ticket);
            Assert.Equal("575354", response.AuthorizationCode);
            Assert.Equal(10000, response.Amount);
            Assert.Equal(6590, response.Last4Digits);
            Assert.Equal(34, response.OperationNumber);
            Assert.Equal("CR", response.CardType);
            Assert.Null(response.AccountingDate);
            Assert.Equal(string.Empty, response.AccountNumber);
            Assert.Equal("VI", response.CardBrand);
            Assert.Equal(new DateTime(2026, 3, 17, 11, 50, 6), response.RealDate);
            Assert.Single(response.PrintingField);
            Assert.Equal(string.Empty, response.PrintingField[0]);
            Assert.Equal(3, response.SharesType);
            Assert.Equal(3, response.SharesNumber);
            Assert.Equal(3334, response.SharesAmount);
            Assert.Equal("CUOTAS SIN INTERES", response.SharesTypeGloss);
            Assert.Contains("Function: 0260", lastSaleResponseText);
            Assert.Contains("Response code:0", lastSaleResponseText);
            Assert.Contains("Card Type: CR", lastSaleResponseText);
            Assert.Contains("Card Brand: VI", lastSaleResponseText);
            Assert.Contains("Shares Type: 3", lastSaleResponseText);
            Assert.Contains("Shares Type Gloss: CUOTAS SIN INTERES", lastSaleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
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
            Assert.Contains("Function: 0260", lastSaleResponseText);
            Assert.Contains("Response code:11", lastSaleResponseText);
            Assert.Contains("Response message: No existe venta", lastSaleResponseText);
            Assert.Contains("Shares Type: -1", lastSaleResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithVoucher_WhenThereAreCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|1";
            const string responsePayload =
                "0510|00|597029414300|IM750164|" +
                "    REPORTE DEL CIERRE DEL TERMINAL                       Tbk                                     MATI                                  Santiago                " +
                "               11111111-1                               SANTIAGO                          597029414300-M261L1           FECHA             HORA          TERMINAL" +
                "17/03/26        12:06:39        IM750164                                                       NUMERO              TOTAL" +
                "VISA             002             $20.000----------------------------------------TOTAL CAPTURAS   002             $20.000";

            var task = _pos.Close(sendVoucher: true);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            CloseResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string closeResponseText = response.ToString();

            Assert.Equal("0510", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.NotEmpty(response.PrintingField);
            Assert.Contains("REPORTE DEL CIERRE DEL TERMINAL", voucher);
            Assert.Contains("NUMERO              TOTAL", voucher);
            Assert.Contains("VISA", voucher);
            Assert.Contains("TOTAL CAPTURAS", voucher);
            Assert.Contains("$20.000", voucher);
            Assert.Contains("Function: 0510", closeResponseText);
            Assert.Contains("Response code:0", closeResponseText);
            Assert.Contains("Printing Field:", closeResponseText);
            Assert.Contains("TOTAL CAPTURAS", closeResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
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
            Assert.Contains("Function: 0510", closeResponseText);
            Assert.Contains("Response code:0", closeResponseText);
            Assert.Contains("Printing Field:", closeResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
        }

        [Fact]
        public async Task Close_ShouldParseApprovedResponseWithVoucher_WhenThereAreNoCapturedTransactions()
        {
            const string expectedCommandPayload = "0500|1";
            const string responsePayload =
                "0510|00|597029414300|IM750164|" +
                "    REPORTE DEL CIERRE DEL TERMINAL                       Tbk                                     MATI                                  Santiago                " +
                "               11111111-1                               SANTIAGO                          597029414300-M261L1           FECHA             HORA          TERMINAL" +
                "17/03/26        12:08:10        IM750164                                                       NUMERO              TOTAL" +
                "----------------------------------------TOTAL CAPTURAS   000                  $0";

            var task = _pos.Close(sendVoucher: true);

            Assert.Equal(BuildCommandFrame(expectedCommandPayload), _mockHandler.WrittenData.Single());

            _mockHandler.SimulateIncoming(ACK.ToString());
            _mockHandler.SimulateIncoming(BuildResponseFrame(responsePayload));

            CloseResponse response = await task;
            string voucher = string.Concat(response.PrintingField);
            string closeResponseText = response.ToString();

            Assert.Equal("0510", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.Success);
            Assert.Equal(597029414300, response.CommerceCode);
            Assert.Equal("IM750164", response.TerminalId);
            Assert.NotEmpty(response.PrintingField);
            Assert.Contains("REPORTE DEL CIERRE DEL TERMINAL", voucher);
            Assert.Contains("NUMERO              TOTAL", voucher);
            Assert.DoesNotContain("VISA", voucher);
            Assert.Contains("TOTAL CAPTURAS", voucher);
            Assert.Contains("$0", voucher);
            Assert.Contains("Function: 0510", closeResponseText);
            Assert.Contains("Response code:0", closeResponseText);
            Assert.Contains("Printing Field:", closeResponseText);
            Assert.Contains("TOTAL CAPTURAS   000", closeResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
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
            Assert.Contains("Function: 0510", closeResponseText);
            Assert.Contains("Response code:0", closeResponseText);
            Assert.Contains("Printing Field:", closeResponseText);
            Assert.Equal(2, _mockHandler.WrittenData.Count);
            Assert.Equal(ACK.ToString(), _mockHandler.WrittenData.Last());
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
    }
}
