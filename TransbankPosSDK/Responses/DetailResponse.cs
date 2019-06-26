using System;
using System.Collections.Generic;
using System.Globalization;
using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class DetailResponse
    {
        private readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Command", 0 },
            { "ResponseCode", 1 },
            { "CommerceCode", 2 },
            { "TerminalID", 3 },
            { "Ticket", 4 },
            { "AutorizationCode", 5 },
            { "Ammount", 6 },
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
            { "FeeAmmount", 17 },
            { "FeeNumber", 18 }
        };

        public int Command { set; get; }
        public int ResponseCode { set; get; }
        public int CommerceCode { set; get; }
        public int Ticket { set; get; }
        public int AutorizationCode { set; get; }
        public int Ammount { set; get; }
        public int Last4Digits { set; get; }
        public int OperationNumber { set; get; }
        public string CardType { set; get; }
        public DateTime? AccountingDate { set; get; }
        public long AccountNumber { set; get; }
        public string CardBrand { set; get; }
        public DateTime? RealDate { set; get; }
        public int EmployeeId { set; get; }
        public int Tip { set; get; }
        public int FeeAmount { set; get; }
        public int FeeNumber { set; get; }

        public DetailResponse()
        {

        }

        public void Parse(string line) {
            line = line.Substring(1);
            string[] fields = line.Split('|');

            Command = fields[ParameterMap["Command"]].Trim() != "" ? int.Parse(fields[ParameterMap["Command"]]) : 0;
            ResponseCode = fields[ParameterMap["ResponseCode"]].Trim() != "" ? int.Parse(fields[ParameterMap["ResponseCode"]]) : -1;
            CommerceCode = fields[ParameterMap["CommerceCode"]].Trim() != "" ? int.Parse(fields[ParameterMap["CommerceCode"]]) : 0;
            Ticket = fields[ParameterMap["Ticket"]].Trim() != "" ? int.Parse(fields[ParameterMap["Ticket"]]) : 0;
            AutorizationCode = fields[ParameterMap["AutorizationCode"]].Trim() != "" ? int.Parse(fields[ParameterMap["AutorizationCode"]]) : 0;
            Ammount = fields[ParameterMap["Ammount"]].Trim() != "" ? int.Parse(fields[ParameterMap["Ammount"]]) : 0;
            Last4Digits = fields[ParameterMap["Last4Digits"]].Trim() != "" ? int.Parse(fields[ParameterMap["Last4Digits"]]) : 0;
            OperationNumber = fields[ParameterMap["OperationNumber"]].Trim() != "" ? int.Parse(fields[ParameterMap["OperationNumber"]]) : 0;
            CardType = fields[ParameterMap["CardType"]].Trim();

            string date = fields[ParameterMap["AccountingDate"]].Trim();
            AccountingDate = null;
            if (date != "")
            {
                AccountingDate = DateTime.ParseExact(date, "ddMMyyyy", DateTimeFormatInfo.InvariantInfo);
            }

            AccountNumber = fields[ParameterMap["AccountNumber"]].Trim() != "" ? long.Parse(fields[ParameterMap["AccountNumber"]]) : 0;
            CardBrand = fields[ParameterMap["CardBrand"]].Trim();

            date = fields[ParameterMap["RealDate"]].Trim();
            string hour = fields[ParameterMap["RealTime"]].Trim();

            RealDate = null;
            if (date + hour != "")
            {
                RealDate = DateTime.ParseExact(date + hour, "ddMMyyyyHHmmss", DateTimeFormatInfo.InvariantInfo);
            }

            EmployeeId = fields[ParameterMap["EmployeeId"]].Trim() != "" ? int.Parse(fields[ParameterMap["EmployeeId"]]) : 0;
            Tip = fields[ParameterMap["Tip"]].Trim() != "" ? int.Parse(fields[ParameterMap["Tip"]]) : 0;
            FeeAmount = fields[ParameterMap["FeeAmount"]].Trim() != "" ? int.Parse(fields[ParameterMap["FeeAmount"]]) : 0;
            FeeNumber = fields[ParameterMap["FeeNumber"]].Trim() != "" ? int.Parse(fields[ParameterMap["FeeNumber"]]) : 0;
        }
    }
}
