using System;
using System.Collections.Generic;
using Transbank.Utils;
using Transbank.Exceptions.CommonExceptions;
using System.Text;
using Transbank.Responses.CommonResponses;
using Transbank.Responses.AutoservicioResponse;
using Transbank.Exceptions.AutoservicioExceptions;
using System.Threading.Tasks;

namespace Transbank.POSAutoservicio
{
    public class POSAutoservicio : Serial
    {
        public POSAutoservicio()
        {

        }

        public static POSAutoservicio Instance { get; } = new POSAutoservicio();

        public async Task<bool> Poll()
        {
            DiscardBuffer();

            if (CantWrite())
            {
                throw new TransbankException($"Unable to Poll port {Port.PortName} is closed");
            }

            try
            {
                byte[] buffer = new byte[1];
                string command = "0100";

                Console.WriteLine($"Out (Hex): {ToHexString(command)}");
                Console.WriteLine($"Out (ASCII): {command}");

                Port.Write(command);
                await Port.BaseStream.ReadAsync(buffer, 0, 1);
                return CheckACK(buffer[0]);
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

        public async Task<LoadKeysResponse> LoadKeys()
        {
            try
            {
                await WriteData("0800");
                return new LoadKeysResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLoadKeysException("Unable to execute Load Keys in pos", e);
            }
        }

        public async Task<bool> Initialization()
        {
            try
            {
                byte[] buffer = new byte[1];
                string command = "0070";

                Console.WriteLine($"Out (Hex): {ToHexString(command)}");
                Console.WriteLine($"Out (ASCII): {command}");

                Port.Write(command);
                await Port.BaseStream.ReadAsync(buffer, 0, 1);
                return CheckACK(buffer[0]);
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Initialization command on port {Port.PortName}", e);
            }
        }

        public async Task<InitializationResponse> InitializationResponse()
        {
            try
            {
                await WriteData("0080\x0B");
                return new InitializationResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankInitializationResponseException("Unable to execute Initialization Response in pos", e);
            }
        }

        public async Task<SaleResponse> Sale(int amount, string ticket, bool sendVoucher = false, bool sendStatus = false)
        {
            if (amount < 50)
            {
                throw new TransbankSaleException("Amount must be greater than 50.");
            }
            if (amount > 999999999)
            {
                throw new TransbankSaleException("Amount must be less than 999999999.");
            }
            if (ticket.Length > 20)
            {
                throw new TransbankSaleException("The ticket must be up to 20 characters.");
            }
            string message = $"0200|{amount}|{ticket}|{Convert.ToInt32(sendVoucher)}|{Convert.ToInt32(sendStatus)}";
            try
            {
                await WriteData(MessageWithLRC(message), intermediateMessages: sendStatus);
                return new SaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankSaleException($"Unable to execute sale on pos", e);
            }
        }

        public async Task<MultiCodeSaleResponse> MultiCodeSale(int amount, string ticket, long commerceCode = 0, bool sendVoucher = false, bool sendStatus = false)
        {
            if (amount < 50)
            {
                throw new TransbankMultiCodeSaleException("Amount must be greater than 50.");
            }
            if (amount > 999999999)
            {
                throw new TransbankMultiCodeSaleException("Amount must be less than 999999999.");
            }
            if (ticket.Length > 20)
            {
                throw new TransbankMultiCodeSaleException("The ticket must up to 20 characters.");
            }
            string code = commerceCode != 0 ? commerceCode.ToString() : "";
            string message = $"0270|{amount}|{ticket}|{Convert.ToInt32(sendVoucher)}|{Convert.ToInt32(sendStatus)}|{code}";
            try
            {
                await WriteData(MessageWithLRC(message), intermediateMessages: sendStatus);
                return new MultiCodeSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeSaleException($"Unable to execute multicode sale on pos", e);
            }
        }

        public async Task<LastSaleResponse> LastSale(bool sendVoucher = false)
        {
            try
            {
                string message = $"0250|{Convert.ToInt32(sendVoucher)}";
                await WriteData(MessageWithLRC(message));
                return new LastSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLastSaleException($"Unable to recover last sale from pos", e);
            }
        }

        public async Task<RefundResponse> Refund()
        {
            string message = $"1200";

            try
            {
                await WriteData(MessageWithLRC(message));
                return new RefundResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankRefundException("Unable to make Refund on POS", e);
            }
        }

        public async Task<CloseResponse> Close(bool sendVoucher)
        {
            string message = $"0500|{Convert.ToInt32(sendVoucher)}";

            try
            {          
                await WriteData(MessageWithLRC(message));
                return new CloseResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankCloseException("Unable to execute close in pos", e);
            }
        }
    }
}
