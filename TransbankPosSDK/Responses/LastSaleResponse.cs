using System;
using System.Collections.Generic;
using System.Globalization;
using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class LastSaleResponse : LoadKeysResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Ticket", 4},
            { "AutorizationCode", 5},
            { "Ammount", 6},
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

        public string Ticket { set; get; }
        public int AutorizationCode { set; get; }
        public int Ammount { set; get; }
        public int SharesNumber { set; get; }
        public int SharesAmount { set; get; }
        public int Last4Digits { set; get; }
        public int OperationNumber { set; get; }
        public string CardType { set; get; }
        public DateTime? AccountingDate { set; get; }
        public long AccountNumber { set; get; }
        public string CardBrand { set; get; }
        public DateTime? RealDate { set; get; }
        public int EmployeeId { set; get; }
        public int Tip { set; get; }

        public LastSaleResponse(string cresponse) : base(CreateBase(cresponse)) {
            cresponse = cresponse.Substring(1);
            string[] fields = cresponse.Split('|');

            Ticket = fields[ParameterMap["Ticket"]].Trim();
            AutorizationCode = fields[ParameterMap["AutorizationCode"]].Trim() != "" ? int.Parse(fields[ParameterMap["AutorizationCode"]]) : 0;
            Ammount = fields[ParameterMap["Ammount"]].Trim() != "" ? int.Parse(fields[ParameterMap["Ammount"]]) : 0;
            SharesNumber = fields[ParameterMap["SharesNumber"]].Trim() != "" ? int.Parse(fields[ParameterMap["SharesNumber"]]) : 0;
            SharesAmount = fields[ParameterMap["SharesAmount"]].Trim() != "" ? int.Parse(fields[ParameterMap["SharesAmount"]]) : 0;
            Last4Digits = fields[ParameterMap["Last4Digits"]].Trim() != "" ? int.Parse(fields[ParameterMap["Last4Digits"]]) : 0;
            OperationNumber = fields[ParameterMap["OperationNumber"]].Trim() != "" ? int.Parse(fields[ParameterMap["OperationNumber"]]) : 0;
            CardType = fields[ParameterMap["CardType"]].Trim();

            string date = fields[ParameterMap["AccountingDate"]].Trim();
            AccountingDate = null;
            if (date != "")
            {
                DateTime parsedDate = new DateTime();
                DateTime.TryParseExact(date, "ddMMyyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out parsedDate);
                AccountingDate = parsedDate;
            }

            string accountString = fields[ParameterMap["AccountNumber"]].Trim().Replace("*", "");
            AccountNumber = accountString != "" ? long.Parse(accountString) : 0;
            CardBrand = fields[ParameterMap["CardBrand"]].Trim();

            date = fields[ParameterMap["RealDate"]].Trim();
            string hour = fields[ParameterMap["RealTime"]].Trim();

            RealDate = null;
            if (date + hour != "")
            {
                DateTime parsedDate = new DateTime();
                DateTime.TryParseExact(date + hour, "ddMMyyyyHHmmss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out parsedDate);
                RealDate = parsedDate;
            }

            EmployeeId = fields[ParameterMap["EmployeeId"]].Trim() != "" ? int.Parse(fields[ParameterMap["EmployeeId"]]) : 0;
            Tip = fields[ParameterMap["Tip"]].Trim() != "" ? int.Parse(fields[ParameterMap["Tip"]]) : 0;
        }

        public override string ToString()
        {
            string formatedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formatedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            return "Function: " + FunctionCode + "\n" +
                   "Response: "+ ResponseMessage + "\n" +
                   "Commerce Code: " + CommerceCode + "\n" +
                   "Terminal Id: " + TerminalId + "\n" +
                   "Ticket: " + Ticket + "\n" +
                   "Autorization Code: " + AutorizationCode + "\n" +
                   "Ammount: " + Ammount + "\n" +
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

        private static BaseResponse CreateBase(string cresponse)
        {
            return new BaseResponse
            {
                function = int.Parse(cresponse.Substring(1, 4)),
                responseCode = int.Parse(cresponse.Substring(6, 2)),
                commerceCode = long.Parse(cresponse.Substring(9, 12)),
                terminalId = cresponse.Substring(22, 8)
            };
        }
    }
}
