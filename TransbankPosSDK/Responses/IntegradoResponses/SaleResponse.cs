using System;
using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.IntegradoResponses
{
    public class SaleResponse : CommonResponses.LoadKeysResponse
    {
        public string Ticket { get; }
        public string AuthorizationCode { get; }
        public int? Amount { get; }
        public int? InstallmentsNumber { get; }
        public int? InstallmentsAmount { get; }
        public int? Last4Digits { get; }
        public int? OperationNumber { get; }
        public string CardType { get; }
        public DateTime? AccountingDate { get; }
        public string AccountNumber { get; }
        public string CardBrand { get; }
        public DateTime? RealDate { get; }
        public int? EmployeeId { get; }
        public int? Tip { get; }
        public string RawVoucher { get; }
        public List<string> PrintingField => VoucherParser.ParsePrintingField(RawVoucher);

        public SaleResponse(string response) : base(response)
        {
            Ticket = GetOptionalStringSegment(4);
            AuthorizationCode = GetOptionalStringSegment(5);
            Amount = GetOptionalIntSegment(6, "Amount");
            InstallmentsNumber = GetOptionalIntSegment(7, "InstallmentsNumber");
            InstallmentsAmount = GetOptionalIntSegment(8, "InstallmentsAmount");
            Last4Digits = GetOptionalIntSegment(9, "Last4Digits");
            OperationNumber = GetOptionalIntSegment(10, "OperationNumber");
            CardType = GetOptionalStringSegment(11);
            AccountingDate = GetOptionalDateSegment(12, "ddMMyyyy", "AccountingDate");
            AccountNumber = GetOptionalStringSegment(13);
            CardBrand = GetOptionalStringSegment(14);
            RealDate = GetOptionalCombinedDateTimeSegment(15, 16, "ddMMyyyyHHmmss", "RealDate");
            EmployeeId = GetOptionalIntSegment(17, "EmployeeId");
            Tip = GetOptionalIntSegment(18, "Tip");
            RawVoucher = VoucherParser.ExtractRawVoucher(Response, 19);
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
                   "Installments Number: " + InstallmentsNumber + "\n" +
                   "Installments Amount: " + InstallmentsAmount + "\n" +
                   "Last 4 Digits: " + Last4Digits + "\n" +
                   "Operation Number: " + OperationNumber + "\n" +
                   "Card Type: " + CardType + "\n" +
                   "Accounting Date: " + formattedAccountingDate + "\n" +
                   "Account Number: " + AccountNumber + "\n" +
                   "Card Brand: " + CardBrand + "\n" +
                   "Real Date: " + formattedRealDate + "\n" +
                   "Employee Id: " + EmployeeId + "\n" +
                   "Tip: " + Tip + "\n" +
                   "Printing Field: " + "\n" + printingFieldText;
        }
    }
}
