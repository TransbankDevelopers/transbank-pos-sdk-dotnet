using System;
using System.Collections.Generic;

namespace Transbank.Utils
{
    internal static class VoucherParser
    {
        private const int VoucherLineLength = 40;

        internal static string ExtractRawVoucher(string response, int fieldIndex)
        {
            try
            {
                string[] arrayResponse = response.Split('|');
                if (arrayResponse.Length <= fieldIndex)
                {
                    return string.Empty;
                }

                return arrayResponse[fieldIndex];
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
        }

        internal static List<string> ParsePrintingField(string rawVoucher)
        {
            List<string> printingField = new List<string>();

            if (string.IsNullOrWhiteSpace(rawVoucher) || rawVoucher.Length % VoucherLineLength != 0)
            {
                return printingField;
            }

            for (int i = 0; i < rawVoucher.Length; i += VoucherLineLength)
            {
                printingField.Add(rawVoucher.Substring(i, VoucherLineLength));
            }

            return printingField;
        }

        internal static string FormatPrintingField(IReadOnlyList<string> printingField)
        {
            if (printingField.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("\r\n", printingField);
        }
    }
}
