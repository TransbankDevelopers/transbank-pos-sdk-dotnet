using System;
using System.Globalization;
using System.Collections.Generic;

namespace Transbank.Responses.AutoservicioResponse
{
    public class SaleResponse : CommonResponses.LoadKeysResponse
    {
        protected Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Ticket", 4},
            { "AuthorizationCode", 5},
            { "Amount", 6},
            { "Last4Digits", 7},
            { "OperationNumber", 8},
            { "CardType", 9},
            { "AccountingDate", 10},
            { "AccountNumber", 11},
            { "CardBrand", 12},
            { "RealDate", 13},
            { "RealTime", 14},
            { "PrintingField", 15},
            { "SharesType", 16},
            { "SharesNumber", 17},
            { "SharesAmount", 18},
            { "SharesTypeGloss", 19}
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
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["Amount"]].Trim(), out int amount);
                    return amount;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public int Last4Digits
        {
            get
            {            
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["Last4Digits"]].Trim(), out int last4Digits);
                    return last4Digits;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public int OperationNumber
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["OperationNumber"]].Trim(), out int operationNumber);
                    return operationNumber;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
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
        public List<string> PrintingField
        {
            get
            {
                List<string> printingField = new List<string>();

                try
                {
                    string[] arrayResponse = Response.Split('|');
                    if (Response.Split('|').Length < 5)
                    {
                        printingField.Add("");
                        return printingField;
                    }

                    string response = arrayResponse[ParameterMap["PrintingField"]];

                    if (response.Length % 40 != 0)
                    {
                        printingField.Add(response);
                        return printingField;
                    }

                    for (int i = 0; i < response.Length; i += 40)
                        printingField.Add(response.Substring(i, 40));

                    return printingField;
                }
                catch (IndexOutOfRangeException)
                {
                    return printingField;
                }
            }
        }
        public int SharesType
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["SharesType"]].Trim(), out int SharesType);
                    return SharesType;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public int SharesNumber
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["SharesNumber"]].Trim(), out int SharesNumber);
                    return SharesNumber;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public int SharesAmount
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["SharesAmount"]].Trim(), out int sharesAmount);
                    return sharesAmount;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public string SharesTypeGloss
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["SharesTypeGloss"]].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    return "";
                }
            }
        }

        public SaleResponse(string response) : base(response) { }



        public override string ToString()
        {
            string formatedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formatedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            return base.ToString() + "\n" +
                   "Ticket: " + Ticket + "\n" +
                   "AuthorizationCode Code: " + AuthorizationCode + "\n" +
                   "Amount: " + Amount + "\n" +
                   "Last 4 Digits: " + Last4Digits + "\n" +
                   "Operation Number: " + OperationNumber + "\n" +
                   "Card Type: " + CardType + "\n" +
                   "Accounting Date: " + formatedAccountingDate + "\n" +
                   "Account Number: " + AccountNumber + "\n" +
                   "Card Brand: " + CardBrand + "\n" +
                   "Real Date: " + formatedRealDate + "\n" +
                   "Printing Field: " + ((PrintingField.Count > 1) ? "\r\n" + string.Join("\r\n", PrintingField) : PrintingField[0])  + "\n" +
                   "Shares Type: " + SharesType + "\n" +
                   "Shares Number: " + SharesNumber + "\n" +
                   "Shares Amount: " + SharesAmount + "\n" +
                   "Shares Type Gloss: " + SharesTypeGloss;
        }
    }
}
