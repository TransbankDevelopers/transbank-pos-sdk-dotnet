using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.AutoservicioResponse
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        public long? CommerceProviderCode { get; }
        public new string RawVoucher { get; }
        public new List<string> PrintingField => VoucherParser.ParsePrintingField(RawVoucher);
        public new int? InstallmentsType { get; }
        public new int? InstallmentsNumber { get; }
        public new int? InstallmentsAmount { get; }
        public new string InstallmentsTypeDescription { get; }

        public MultiCodeSaleResponse(string response) : base(response)
        {
            CommerceProviderCode = GetOptionalLongSegment(15, "CommerceProviderCode");
            RawVoucher = VoucherParser.ExtractRawVoucher(Response, 16);
            InstallmentsType = GetOptionalIntSegment(17, "InstallmentsType");
            InstallmentsNumber = GetOptionalIntSegment(18, "InstallmentsNumber");
            InstallmentsAmount = GetOptionalIntSegment(19, "InstallmentsAmount");
            InstallmentsTypeDescription = GetOptionalStringSegment(20);
        }

        public override string ToString()
        {
            string formattedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formattedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string printingFieldText = VoucherParser.FormatPrintingField(PrintingField);
            return "Function: " + FunctionCode + "\n" +
                   "Response code:" + ResponseCode + "\n" +
                   "Response: " + ResponseMessage + "\n" +
                   "Commerce Code: " + CommerceCode + "\n" +
                   "Terminal Id: " + TerminalId + "\n" +
                   "Ticket: " + Ticket + "\n" +
                   "AuthorizationCode Code: " + AuthorizationCode + "\n" +
                   "Amount: " + Amount + "\n" +
                   "Last 4 Digits: " + Last4Digits + "\n" +
                   "Operation Number: " + OperationNumber + "\n" +
                   "Card Type: " + CardType + "\n" +
                   "Accounting Date: " + formattedAccountingDate + "\n" +
                   "Account Number: " + AccountNumber + "\n" +
                   "Card Brand: " + CardBrand + "\n" +
                   "Real Date: " + formattedRealDate + "\n" +
                   "CommerceProviderCode: " + CommerceProviderCode + "\n" +
                   "Printing Field: " + "\n" + printingFieldText + "\n" +
                   "Installments Type: " + InstallmentsType + "\n" +
                   "Installments Number: " + InstallmentsNumber + "\n" +
                   "Installments Amount: " + InstallmentsAmount + "\n" +
                   "Installments Type Description: " + InstallmentsTypeDescription;
        }
    }
}
