using System;
using System.Globalization;
using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.IntegradoResponses
{
    public class SaleResponse : CommonResponses.LoadKeysResponse
    {
        protected Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Ticket", 4},
            { "AuthorizationCode", 5},
            { "Amount", 6},
            { "InstallmentsNumber", 7},
            { "InstallmentsAmount", 8},
            { "Last4Digits", 9},
            { "OperationNumber", 10},
            { "CardType", 11},
            { "AccountingDate", 12},
            { "AccountNumber", 13},
            { "CardBrand", 14},
            { "RealDate", 15},
            { "RealTime", 16},
            { "EmployeeId", 17},
            { "Tip", 18 },
            { "Voucher", 19 }

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
        public int InstallmentsNumber
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["InstallmentsNumber"]].Trim(), out int installmentsNumber);
                return installmentsNumber;
            }
        }
        public int InstallmentsAmount
        {
            get
            {
                int.TryParse(Response.Split('|')[ParameterMap["InstallmentsAmount"]].Trim(), out int installmentsAmount);
                return installmentsAmount;
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

        public List<string> PrintingField
        {
            get
            {
                return VoucherParser.ParsePrintingField(RawVoucher);
            }
        }

        public string RawVoucher
        {
            get
            {
                return VoucherParser.ExtractRawVoucher(Response, ParameterMap["Voucher"]);
            }
        }


        public SaleResponse(string response) : base(response) { }

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
