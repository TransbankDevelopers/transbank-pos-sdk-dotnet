using System;
using System.Globalization;
using System.Collections.Generic;

namespace Transbank.Responses.AutoservicioResponse
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "CommerceProviderCode", 14},
            { "PrintingField", 16},
            { "SharesType", 17},
            { "SharesNumber", 18},
            { "SharesAmount", 19},
            { "SharesTypeGloss", 20}
        };

        public int CommerceProviderCode
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["CommerceProviderCode"]].Trim(), out int commerceProviderCode);
                    return commerceProviderCode;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public new List<string> PrintingField
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
        public new int SharesType
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["SharesType"]].Trim(), out int sharesType);
                    return sharesType;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public new int SharesNumber
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
        public new int SharesAmount
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
        public new string SharesTypeGloss
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
                   "Last 4 Digits: " + Last4Digits + "\n" +
                   "Operation Number: " + OperationNumber + "\n" +
                   "Card Type: " + CardType + "\n" +
                   "Accounting Date: " + formatedAccountingDate + "\n" +
                   "Account Number: " + AccountNumber + "\n" +
                   "Card Brand: " + CardBrand + "\n" +
                   "Real Date: " + formatedRealDate + "\n" +
                   "CommerceProviderCode: " + CommerceProviderCode + "\n" +
                   "Printing Field: " + ((PrintingField.Count > 1) ? "\r\n" + string.Join("\r\n", PrintingField) : PrintingField[0]) + "\n" +
                   "Shares Type: " + SharesType + "\n" +
                   "Shares Number: " + SharesNumber + "\n" +
                   "Shares Amount: " + SharesAmount + "\n" +
                   "Shares Type Gloss: " + SharesTypeGloss;
        }
    }
}
