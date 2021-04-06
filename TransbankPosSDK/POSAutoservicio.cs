using System;
using System.Collections.Generic;
using Transbank.POS.Utils;
using Transbank.POS.Exceptions.CommonExceptions;
using System.Text;
using Transbank.POS.Responses.CommonResponses;
using Transbank.POS.Responses.AutoservicioResponse;
using Transbank.POS.Exceptions.IntegradoExceptions;

namespace Transbank.POSAutoservicio
{
    public class POSAutoservicio : Serial
    {
        public POSAutoservicio()
        {

        }

        public static POSAutoservicio Instance { get; } = new POSAutoservicio();

        public bool Poll()
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
                Console.WriteLine($"Out: {string.Format("0:X2", command)}");
                Port.Write("0100");
                Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

        public LoadKeysResponse LoadKeys()
        {
            try
            {
                WriteData("0800").Wait();
                return new LoadKeysResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLoadKeysException("Unable to execute Load Keys in pos", e);
            }
        }

        public bool Initialization()
        {
            DiscardBuffer();

            if (CantWrite())
            {
                throw new TransbankException($"Unable to Initialization port {Port.PortName} is closed");
            }

            try
            {
                byte[] buffer = new byte[1];
                string command = "0070";

                Console.WriteLine($"Out: {ToHexString(command)}");

                Port.Write(command);
                Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return CheckACK(buffer[0]);
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Initialization command on port {Port.PortName}", e);
            }
        }

        public InitializationResponse InitializationResponse()
        {
            try
            {
                WriteData("0080\x0B").Wait();
                return new InitializationResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLoadKeysException("Unable to execute Load Keys in pos", e);
            }
        }

        public SaleResponse Sale(int amount, string ticket, bool sendVoucher = false, bool sendStatus = false)
        {
            if (amount <= 0)
            {
                throw new TransbankSaleException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            string message = $"0200|{amount}|{ticket}|{Convert.ToInt32(sendVoucher)}|{Convert.ToInt32(sendStatus)}";
            try
            {
                WriteData(MessageWithLRC(message), intermediateMessages: sendStatus).Wait();
                return new SaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankSaleException($"Unable to execute sale on pos", e);
            }
        }
    }
}
