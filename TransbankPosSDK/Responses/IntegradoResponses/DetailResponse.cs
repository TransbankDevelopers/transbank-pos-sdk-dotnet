using System;

namespace Transbank.Responses.IntegradoResponses
{
    public class DetailResponse : SaleResponse
    {
        public new int? Last4Digits { get; }
        public new int? OperationNumber { get; }
        public new string CardType { get; }
        public new DateTime? AccountingDate { get; }
        public new string AccountNumber { get; }
        public new string CardBrand { get; }
        public new DateTime? RealDate { get; }
        public new int? EmployeeId { get; }
        public new int? Tip { get; }
        public new int? InstallmentsAmount { get; }
        public new int? InstallmentsNumber { get; }

        public DetailResponse(string detail) : base(detail)
        {
            Last4Digits = GetOptionalIntSegment(7, "Last4Digits");
            OperationNumber = GetOptionalIntSegment(8, "OperationNumber");
            CardType = GetOptionalStringSegment(9);
            AccountingDate = GetOptionalDateSegment(10, "ddMMyyyy", "AccountingDate");
            AccountNumber = GetOptionalStringSegment(11);
            CardBrand = GetOptionalStringSegment(12);
            RealDate = GetOptionalCombinedDateTimeSegment(13, 14, "ddMMyyyyHHmmss", "RealDate");
            EmployeeId = GetOptionalIntSegment(15, "EmployeeId");
            Tip = GetOptionalIntSegment(16, "Tip");
            InstallmentsAmount = GetOptionalIntSegment(17, "InstallmentsAmount");
            InstallmentsNumber = GetOptionalIntSegment(18, "InstallmentsNumber");
        }

        public override string ToString()
        {
            string formattedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formattedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            return "Function: " + FunctionCode + "\n" +
                   "Response: " + ResponseMessage + "\n" +
                   "Commerce Code: " + CommerceCode + "\n" +
                   "Terminal Id: " + TerminalId + "\n" +
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
                   "Tip: " + Tip;
        }
    }
}
