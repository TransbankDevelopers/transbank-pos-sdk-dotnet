using System;
using System.Globalization;
using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class SaleResponse : LoadKeysResponse
    {
        protected Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Ticket", 4},
            { "AuthorizationCode", 5},
            { "Amount", 6},
            { "SharesNumber", 7},
            { "SharesAmount", 8},
            { "Last4Digits", 9},
            { "OperationNumber", 10},
            { "CardType", 11},
            { "AccountingDate", 12},
            { "AccountNumber", 13},
            { "CardBrand", 14},
            { "RealDate", 15},
            { "RealTime", 16},
            { "EmployeeId", 17},
            { "Tip", 18}
        };

        public string Ticket
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["Ticket"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public string AuthorizationCode
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["AuthorizationCode"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }
        public int Amount
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Amount"]].Trim(), out int amount);
                return amount;
            }
        }
        public int SharesNumber
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["SharesNumber"]].Trim(), out int SharesNumber);
                return SharesNumber;
            }
        }
        public int SharesAmount
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["SharesAmount"]].Trim(), out int sharesAmount);
                return sharesAmount;
            }
        }
        public int Last4Digits
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Last4Digits"]].Trim(), out int last4Digits);
                return last4Digits;
            }
        }
        public int OperationNumber
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["OperationNumber"]].Trim(), out int operationNumber);
                return operationNumber;
            }
        }
        public string CardType
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
        public DateTime? AccountingDate
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
        public string AccountNumber
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
        public string CardBrand
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
        public DateTime? RealDate
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
        public int EmployeeId
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["EmployeeId"]].Trim(), out int employeeId);
                return employeeId;
            }
        }
        public int Tip
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["Tip"]].Trim(), out int tip);
                return tip;
            }
        }

        public SaleResponse(string response) : base(response) { }

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
                   "Tip: " + Tip;
        }
    }
}
