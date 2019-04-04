using Transbank.POS.Utils;
using Transbank.POS.Responses;
using Transbank.POS.Exceptions;
using System;

namespace Transbank.POS
{
    public class POS
    {
        public string Port { get; set; }
        private bool _configured = false;

        public void OpenPort(string portName, TbkBaudrate baudrate)
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
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public RegisterCloseResponse RegisterClose()
        {
            if (_configured)
                try
                {
                    RegisterCloseResponse response = new RegisterCloseResponse(TransbankWrap.register_close());
                    if (response.Sucess)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankRegisterCloseException("Register Close retured an error: " + response.Result, response);
                    }
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute register close in pos", e);
                }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public LoadKeysResponse LoadKeys()
        {
            if (_configured)
                try
                {
                    LoadKeysResponse response = new LoadKeysResponse(TransbankWrap.load_keys());
                    if (response.Sucess)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankLoadKeysException("Load Keys retured an error: " + response.Result, response);
                    }
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to load Keys in pos", e);
                }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public bool ClosePort()
        {
            try
            {
                if (TransbankWrap.close_port() == TbkReturn.TBK_OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new TransbankException("Unable to close port: " + Port, e);
            }
        }
    }
}
