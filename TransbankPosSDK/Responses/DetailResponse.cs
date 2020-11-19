using System;
using System.Collections.Generic;
using System.Globalization;

namespace Transbank.POS.Responses
{
    public class DetailResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Last4Digits", 7 },
            { "OperationNumber", 8 },
            { "CardType", 9 },
            { "AccountingDate", 10 },
            { "AccountNumber", 11 },
            { "CardBrand", 12 },
            { "RealDate", 13 },
            { "RealTime", 14 },
            { "EmployeeId", 15 },
            { "Tip", 16 },
            { "SharesAmount", 17 },
            { "SharesNumber", 18 }
        };

        public DetailResponse(string detail) : base(detail) { }

        public new int Last4Digits
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Last4Digits"]].Trim(), out int last4Digits);
                return last4Digits;
            }
        }
        public new int OperationNumber
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["OperationNumber"]].Trim(), out int operationNumber);
                return operationNumber;
            }
        }
        public new string CardType
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["CardType"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public new DateTime? AccountingDate
        {
            get
            {
                string date = "";
                try
                {
                    date = Response.Split('|')[ParameterMap["AccountingDate"]].Trim();
                }
                catch (IndexOutOfRangeException) { }
                if (date != "")
                {
                    DateTime parsedDate = new DateTime();
                    DateTime.TryParseExact(date, "ddMMyyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out parsedDate);
                    return parsedDate;
                }
                return null;
            }
        }
        public new string AccountNumber
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["AccountNumber"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public new string CardBrand
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["CardBrand"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public new DateTime? RealDate
        {
            get
            {
                string date = "";
                string hour = "";
                try
                {
                    date = Response.Split('|')[ParameterMap["RealDate"]].Trim();
                    hour = Response.Split('|')[ParameterMap["RealTime"]].Trim();
                }
                catch (IndexOutOfRangeException) { }

                if (date + hour != "")
                {
                    DateTime parsedDate = new DateTime();
                    DateTime.TryParseExact(date + hour, "ddMMyyyyHHmmss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out parsedDate);
                    return parsedDate;
                }
                return null;
            }
        }
        public new int EmployeeId
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["EmployeeId"]].Trim(), out int employeeId);
                return employeeId;
            }
        }
        public new int Tip
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Tip"]].Trim(), out int tip);
                return tip;
            }
        }
        public new int SharesAmount
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["SharesAmount"]].Trim(), out int sharesAmount);
                return sharesAmount;
            }
        }
        public new int SharesNumber
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["SharesNumber"]].Trim(), out int SharesNumber);
                return SharesNumber;
            }
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
                   "Shares Number: " + SharesNumber + "\n" +
                   "Shares Amount: " + SharesAmount + "\n" +
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
