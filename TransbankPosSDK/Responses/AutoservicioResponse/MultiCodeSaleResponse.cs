using System;
using System.Globalization;
using System.Collections.Generic;

namespace Transbank.Responses.AutoservicioResponse
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "CommerceProviderCode", 15},
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
                string rawVoucher = RawVoucher;

                if (string.IsNullOrWhiteSpace(rawVoucher) || rawVoucher.Length % 40 != 0)
                {
                    return printingField;
                }

                for (int i = 0; i < rawVoucher.Length; i += 40)
                    printingField.Add(rawVoucher.Substring(i, 40));

                return printingField;
            }
        }
        public new string RawVoucher
        {
            get
            {
                try
                {
                    string[] arrayResponse = Response.Split('|');
                    if (arrayResponse.Length <= ParameterMap["PrintingField"])
                    {
                        return string.Empty;
                    }

                    return arrayResponse[ParameterMap["PrintingField"]];
                }
                catch (IndexOutOfRangeException)
                {
                    return string.Empty;
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
            string printingFieldText = PrintingField.Count == 0
                ? ""
                : string.Join("\r\n", PrintingField);
            return "Function: " + FunctionCode + "\n" +
                   "Response code:" + ResponseCode + "\n" +
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
                   "Printing Field: " + "\n" + printingFieldText + "\n" +
                   "Shares Type: " + SharesType + "\n" +
                   "Shares Number: " + SharesNumber + "\n" +
                   "Shares Amount: " + SharesAmount + "\n" +
                   "Shares Type Gloss: " + SharesTypeGloss;
        }
    }
}
