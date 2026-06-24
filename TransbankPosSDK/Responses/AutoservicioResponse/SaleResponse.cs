using System;
using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.AutoservicioResponse
{
    public class SaleResponse : CommonResponses.LoadKeysResponse
    {
        public string Ticket { get; }
        public string AuthorizationCode { get; }
        public int? Amount { get; }
        public int? Last4Digits { get; }
        public int? OperationNumber { get; }
        public string CardType { get; }
        public DateTime? AccountingDate { get; }
        public string AccountNumber { get; }
        public string CardBrand { get; }
        public DateTime? RealDate { get; }
        public string RawVoucher { get; }
        public List<string> PrintingField => VoucherParser.ParsePrintingField(RawVoucher);
        public int? InstallmentsType { get; }
        public int? InstallmentsNumber { get; }
        public int? InstallmentsAmount { get; }
        public string InstallmentsTypeDescription { get; }

        public SaleResponse(string response) : base(response)
        {
            Ticket = GetOptionalStringSegment(4);
            AuthorizationCode = GetOptionalStringSegment(5);
            Amount = GetOptionalIntSegment(6, "Amount");
            Last4Digits = GetOptionalIntSegment(7, "Last4Digits");
            OperationNumber = GetOptionalIntSegment(8, "OperationNumber");
            CardType = GetOptionalStringSegment(9);
            AccountingDate = GetOptionalDateSegment(10, "ddMMyyyy", "AccountingDate");
            AccountNumber = GetOptionalStringSegment(11);
            CardBrand = GetOptionalStringSegment(12);
            RealDate = GetOptionalCombinedDateTimeSegment(13, 14, "ddMMyyyyHHmmss", "RealDate");
            RawVoucher = VoucherParser.ExtractRawVoucher(Response, 15);
            InstallmentsType = GetOptionalIntSegment(16, "InstallmentsType");
            InstallmentsNumber = GetOptionalIntSegment(17, "InstallmentsNumber");
            InstallmentsAmount = GetOptionalIntSegment(18, "InstallmentsAmount");
            InstallmentsTypeDescription = GetOptionalStringSegment(19);
        }

        public override string ToString()
        {
            string formattedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formattedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string printingFieldText = VoucherParser.FormatPrintingField(PrintingField);
            return base.ToString() + "\n" +
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
                   "Printing Field: " + "\n" + printingFieldText + "\n" +
                   "Installments Type: " + InstallmentsType + "\n" +
                   "Installments Number: " + InstallmentsNumber + "\n" +
                   "Installments Amount: " + InstallmentsAmount + "\n" +
                   "Installments Type Description: " + InstallmentsTypeDescription;
        }
    }
}
