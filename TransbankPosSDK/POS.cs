using Transbank.POS.Utils;
using Transbank.POS.Exceptions;
using System;

namespace Transbank.POS
{
    public class POS
    {
        public string Port { get; set; }
        private bool _configured = false;

        public void OpenPort( string portName, TbkBaudrate baudrate)
        {
            try
            {
                if (TransbankWrap.open_port(portName, (int)baudrate) == TbkReturn.TBK_OK)
                    _configured = true;
                else
                    throw new TransbankException("Unable to Open selected port: " + portName);
            }
            catch (Exception e)
            {
                throw new TransbankException("Unable to Open selected port: " + portName, e);
            }
        }

        public bool Polling()
        {
            if (_configured)
                try
                {
                    return TransbankWrap.polling() == TbkReturn.TBK_OK;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to locate POS", e);
                }
            else
                throw new TransbankException("Port not Configured");
        }
    }
}
