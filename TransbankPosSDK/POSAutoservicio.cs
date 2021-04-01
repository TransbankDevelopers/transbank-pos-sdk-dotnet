﻿using System;
using System.Collections.Generic;
using Transbank.POS.Utils;
using Transbank.POS.IntegradoResponses;
using Transbank.POS.IntegradoExceptions;
using Transbank.POS.CommonResponses;
using Transbank.POS.CommonExceptions;
using System.Text;

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
                string command = "0070\x04";

                Console.WriteLine($"Out: {BitConverter.ToString(Encoding.Default.GetBytes(command))}");

                Port.Write(command);
                Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Initialization command on port {Port.PortName}", e);
            }
        }
    }
}
