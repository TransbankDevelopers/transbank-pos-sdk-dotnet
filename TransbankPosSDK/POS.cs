using Transbank.POS.Wrapper;
using Transbank.POS.Exceptions;
using System;

namespace Transbank.POS
{
    public class POS
    {
        public string Port { get; set; }
        private bool _configured = false;

        private void SelectPort(string portName)
        {
            try
            {
                if (TransbankWrap.select_port(portName) == tbk_return.TBK_NOK)
                    throw new TransbankException("Unable to select specified port: " + portName);
            }
            catch (Exception e)
            {
                throw new TransbankException("Unable to select specified port: " + portName, e);
            }
        }

        private void ConfigurePortBaudrate(tbk_baudrate baudrate)
        {
            try
            {
                if (TransbankWrap.configure_port((int)baudrate) == tbk_return.TBK_NOK)
                    throw new TransbankException("Unable to set the specified baudrate: " + baudrate);
            }
            catch (Exception e)
            {
                throw new TransbankException("Unable to set the specified baudrate: " + baudrate, e);
            }
        }

        public void OpenPort( string portName, tbk_baudrate baudrate)
        {
            try
            {
                SelectPort(portName);
                ConfigurePortBaudrate(baudrate);
                if (TransbankWrap.open_configured_port() == tbk_return.TBK_OK)
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
                    return TransbankWrap.polling() == tbk_return.TBK_OK;
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
