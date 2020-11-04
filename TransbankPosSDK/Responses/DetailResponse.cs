using System.Collections.Generic;

namespace Transbank.POS.Responses
{
    public class DetailResponse : SaleResponse
    {
        protected new readonly Dictionary<string, int> ParameterMap = new Dictionary<string, int>
        {
            { "Ticket", 4 },
            { "AuthorizationCode", 5 },
            { "Amount", 6 },
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
    }
}
