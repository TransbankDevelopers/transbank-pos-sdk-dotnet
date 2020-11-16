using System;
using System.Globalization;
using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Filler", 19},
            { "Change", 20},
            { "CommerceProviderCode", 21 }
        };

        public string Filler
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["Filler"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public int Change
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Change"]].Trim(), out int amount);
                return amount;
            }
        }
        public long CommerceProviderCode
        {
            get
            {
                long.TryParse(Response.Split('|')[ParameterMap["CommerceProviderCode"]].Trim(), out long SharesNumber);
                return SharesNumber;
            }
        }

        public MultiCodeSaleResponse(string response) : base(response) { }

        public override string ToString()
        {
            string formatedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formatedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            return "Function: " + FunctionCode + "\n" +
                   "Response: " + ResponseMessage + "\n" +
                   "Commerce Code: " + CommerceCode + "\n" +
                   "Terminal Id: " + TerminalId + "\n" +
                   "Ticket: " + Ticket + "\n" +
                   "AuthorizationCode Code: " + AuthorizationCode + "\n" +
                   "Amount: " + Amount + "\n" +
                   "Shares Number: " + SharesNumber + "\n" +
                   "Shares Amount: " + SharesAmount + "\n" +
                   "Last 4 Digits: " + Last4Digits + "\n" +
                   "Operation Number: " + OperationNumber + "\n" +
                   "Card Type: " + CardType + "\n" +
                   "Accounting Date: " + formatedAccountingDate + "\n" +
                   "Account Number: " + AccountNumber + "\n" +
                   "Card Brand: " + CardBrand + "\n" +
                   "Real Date: " + formatedRealDate + "\n" +
                   "Employee Id: " + EmployeeId + "\n" +
                   "Tip: " + Tip + "\n" +
                   "Change: " + Change + "\n" +
                   "Commerce Provider Code: " + CommerceProviderCode;
        }
    }
}
