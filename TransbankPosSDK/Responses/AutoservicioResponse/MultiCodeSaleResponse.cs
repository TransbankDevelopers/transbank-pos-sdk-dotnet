using System;
using System.Globalization;
using System.Collections.Generic;
using Transbank.Utils;

namespace Transbank.Responses.AutoservicioResponse
{
    public class MultiCodeSaleResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "CommerceProviderCode", 15},
            { "PrintingField", 16},
            { "InstallmentsType", 17},
            { "InstallmentsNumber", 18},
            { "InstallmentsAmount", 19},
            { "InstallmentsTypeDescription", 20}
        };

        public long CommerceProviderCode
        {
            get
            {
                try
                {
                    long.TryParse(Response.Split('|')[ParameterMap["CommerceProviderCode"]].Trim(), out long commerceProviderCode);
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
                return VoucherParser.ParsePrintingField(RawVoucher);
            }
        }
        public new string RawVoucher
        {
            get
            {
                return VoucherParser.ExtractRawVoucher(Response, ParameterMap["PrintingField"]);
            }
        }
        public new int InstallmentsType
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["InstallmentsType"]].Trim(), out int installmentsType);
                    return installmentsType;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public new int InstallmentsNumber
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["InstallmentsNumber"]].Trim(), out int installmentsNumber);
                    return installmentsNumber;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public new int InstallmentsAmount
        {
            get
            {
                try
                {
                    int.TryParse(Response.Split('|')[ParameterMap["InstallmentsAmount"]].Trim(), out int installmentsAmount);
                    return installmentsAmount;
                }
                catch (IndexOutOfRangeException)
                {
                    return -1;
                }
            }
        }
        public new string InstallmentsTypeDescription
        {
            get
            {
                try
                {
                    return Response.Split('|')[ParameterMap["InstallmentsTypeDescription"]].Trim();
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
            string formattedAccountingDate = AccountingDate.HasValue ? AccountingDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string formattedRealDate = RealDate.HasValue ? RealDate.Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
            string printingFieldText = VoucherParser.FormatPrintingField(PrintingField);
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
                   "Accounting Date: " + formattedAccountingDate + "\n" +
                   "Account Number: " + AccountNumber + "\n" +
                   "Card Brand: " + CardBrand + "\n" +
                   "Real Date: " + formattedRealDate + "\n" +
                   "CommerceProviderCode: " + CommerceProviderCode + "\n" +
                   "Printing Field: " + "\n" + printingFieldText + "\n" +
                   "Installments Type: " + InstallmentsType + "\n" +
                   "Installments Number: " + InstallmentsNumber + "\n" +
                   "Installments Amount: " + InstallmentsAmount + "\n" +
                   "Installments Type Description: " + InstallmentsTypeDescription;
        }
    }
}
