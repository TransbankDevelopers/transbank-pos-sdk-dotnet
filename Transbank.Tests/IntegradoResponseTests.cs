using System;
using Transbank.Responses.IntegradoResponses;
using Xunit;

namespace Transbank.Tests
{
    public class IntegradoResponseTests
    {
        private const string VoucherLineOne = "1234567890123456789012345678901234567890";
        private const string VoucherLineTwo = "ABCDEFGHIJABCDEFGHIJABCDEFGHIJABCDEFGHIJ";
        private const string ValidVoucher = VoucherLineOne + VoucherLineTwo;

        [Fact]
        public void SaleResponse_ShouldParseInstallmentsVoucherAndToString()
        {
            string response = "0210|00|597029414300|IT750050|abc123|757752|12000|03|3334|9480|000135|CR|22102025|0000000000000000000|MC|22102025|114016|7|500|" + ValidVoucher;

            var saleResponse = new SaleResponse(response);
            string saleResponseText = saleResponse.ToString();

            Assert.Equal("0210", saleResponse.FunctionCode);
            Assert.Equal(0, saleResponse.ResponseCode);
            Assert.Equal(597029414300, saleResponse.CommerceCode);
            Assert.Equal("IT750050", saleResponse.TerminalId);
            Assert.Equal("abc123", saleResponse.Ticket);
            Assert.Equal("757752", saleResponse.AuthorizationCode);
            Assert.Equal(12000, saleResponse.Amount);
            Assert.Equal(3, saleResponse.InstallmentsNumber);
            Assert.Equal(3334, saleResponse.InstallmentsAmount);
            Assert.Equal(9480, saleResponse.Last4Digits);
            Assert.Equal(135, saleResponse.OperationNumber);
            Assert.Equal("CR", saleResponse.CardType);
            Assert.Equal(new DateTime(2025, 10, 22), saleResponse.AccountingDate);
            Assert.Equal("0000000000000000000", saleResponse.AccountNumber);
            Assert.Equal("MC", saleResponse.CardBrand);
            Assert.Equal(new DateTime(2025, 10, 22, 11, 40, 16), saleResponse.RealDate);
            Assert.Equal(7, saleResponse.EmployeeId);
            Assert.Equal(500, saleResponse.Tip);
            Assert.Equal(ValidVoucher, saleResponse.RawVoucher);
            Assert.Equal(2, saleResponse.PrintingField.Count);
            Assert.Equal(VoucherLineOne, saleResponse.PrintingField[0]);
            Assert.Equal(VoucherLineTwo, saleResponse.PrintingField[1]);
            Assert.Contains("Function: 0210", saleResponseText);
            Assert.Contains("Response code:0", saleResponseText);
            Assert.Contains("Installments Number: 3", saleResponseText);
            Assert.Contains("Installments Amount: 3334", saleResponseText);
            Assert.Contains("Printing Field:", saleResponseText);
        }

        [Fact]
        public void SaleResponse_ShouldReturnEmptyPrintingField_WhenVoucherLengthIsInvalid()
        {
            string response = "0210|00|597029414300|IT750050|abc123|757752|12000|03|3334|9480|000135|CR|22102025|0000000000000000000|MC|22102025|114016|7|500|INVALID";

            var saleResponse = new SaleResponse(response);

            Assert.Equal("INVALID", saleResponse.RawVoucher);
            Assert.Empty(saleResponse.PrintingField);
        }

        [Fact]
        public void DetailResponse_ShouldParseInstallmentsAndToString()
        {
            string response = "0261|00|597029414300|IT750050|abc123|757752|12000|9480|000135|CR|22102025|0000000000000000000|MC|22102025|114016|7|500|3334|03";

            var detailResponse = new DetailResponse(response);
            string detailResponseText = detailResponse.ToString();

            Assert.Equal(9480, detailResponse.Last4Digits);
            Assert.Equal(135, detailResponse.OperationNumber);
            Assert.Equal("CR", detailResponse.CardType);
            Assert.Equal(new DateTime(2025, 10, 22), detailResponse.AccountingDate);
            Assert.Equal("0000000000000000000", detailResponse.AccountNumber);
            Assert.Equal("MC", detailResponse.CardBrand);
            Assert.Equal(new DateTime(2025, 10, 22, 11, 40, 16), detailResponse.RealDate);
            Assert.Equal(7, detailResponse.EmployeeId);
            Assert.Equal(500, detailResponse.Tip);
            Assert.Equal(3334, detailResponse.InstallmentsAmount);
            Assert.Equal(3, detailResponse.InstallmentsNumber);
            Assert.Contains("Installments Number: 3", detailResponseText);
            Assert.Contains("Installments Amount: 3334", detailResponseText);
        }

        [Fact]
        public void MultiCodeSaleResponse_ShouldParseCommerceProviderCodeAndToString()
        {
            string response = "0271|00|597029414300|IT750050|abc123|757752|12000|03|3334|9480|000135|CR|22102025|0000000000000000000|MC|22102025|114016|7|500|FILLER|0|597012345678";

            var multiCodeSaleResponse = new MultiCodeSaleResponse(response);
            string multiCodeSaleResponseText = multiCodeSaleResponse.ToString();

            Assert.Equal("FILLER", multiCodeSaleResponse.Filler);
            Assert.Equal(0, multiCodeSaleResponse.Change);
            Assert.Equal(597012345678, multiCodeSaleResponse.CommerceProviderCode);
            Assert.Contains("Commerce Provider Code: 597012345678", multiCodeSaleResponseText);
        }

        [Fact]
        public void MultiCodeSaleResponse_ShouldReturnEmptyFiller_WhenFieldIsMissing()
        {
            string response = "0271|00|597029414300|IT750050";

            var multiCodeSaleResponse = new MultiCodeSaleResponse(response);

            Assert.Equal(string.Empty, multiCodeSaleResponse.Filler);
        }

        [Fact]
        public void MultiCodeDetailResponse_ShouldParseChangeAndCommerceProviderCode()
        {
            string response = "0291|00|597029414300|IT750050|abc123|757752|12000|03|3334|9480|000135|CR|22102025|0000000000000000000|MC|22102025|114016|7|500|0|597012345678";

            var multiCodeDetailResponse = new MultiCodeDetailResponse(response);
            string multiCodeDetailResponseText = multiCodeDetailResponse.ToString();

            Assert.Equal(0, multiCodeDetailResponse.Change);
            Assert.Equal(597012345678, multiCodeDetailResponse.CommerceProviderCode);
            Assert.Contains("Commerce Provider Code: 597012345678", multiCodeDetailResponseText);
        }

        [Fact]
        public void MultiCodeLastSaleResponse_ShouldParseVoucherAndCommerceProviderCode()
        {
            string response = "0281|00|597029414300|IT750050|abc123|757752|12000|03|3334|9480|000135|CR|22102025|0000000000000000000|MC|22102025|114016|7|500|" + ValidVoucher + "|0|597012345678";

            var multiCodeLastSaleResponse = new MultiCodeLastSaleResponse(response);
            string multiCodeLastSaleResponseText = multiCodeLastSaleResponse.ToString();

            Assert.Equal(ValidVoucher, multiCodeLastSaleResponse.Voucher);
            Assert.Equal(0, multiCodeLastSaleResponse.Change);
            Assert.Equal(597012345678, multiCodeLastSaleResponse.CommerceProviderCode);
            Assert.Contains("Voucher: " + ValidVoucher, multiCodeLastSaleResponseText);
            Assert.Contains("Commerce Provider Code: 597012345678", multiCodeLastSaleResponseText);
        }
    }
}
