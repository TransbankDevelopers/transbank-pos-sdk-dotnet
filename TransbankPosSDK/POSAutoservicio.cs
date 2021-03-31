using System;
using System.Collections.Generic;
using Transbank.POS.Utils;
using Transbank.POS.IntegradoResponses;
using Transbank.POS.IntegradoExceptions;
using Transbank.POS.CommonResponses;
using Transbank.POS.CommonExceptions;

namespace Transbank.POSAutoservicio
{
    public class POSAutoservicio : Serial
    {
        public POSAutoservicio()
        {

        }

        public static POSAutoservicio Instance { get; } = new POSAutoservicio();

        private void DiscardBuffer()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();
        }

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
                Port.Write("0100");
                Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

    }
}
