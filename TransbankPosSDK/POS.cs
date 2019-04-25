using Transbank.POS.Utils;
using Transbank.POS.Responses;
using Transbank.POS.Exceptions;
using System;

namespace Transbank.POS
{
    public class POS
    {
        private static readonly POS _instance = new POS();
        private bool _configured = false;
        
        public string Port { get; set; }

        private POS()
        {
        }

        public static POS Instance
        {
            get
            {
                return _instance;
            }
        }

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

        public SaleResponse Sale(int amount, int ticket)
        {
            if (_configured)
            {
                try
                {
                    SaleResponse response = new SaleResponse(TransbankWrap.sale(amount, ticket, false));
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankSaleException("Register Close returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute Sale on pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public bool SetNormalMode()
        {
            if (_configured)
            {
                try
                {
                    return TransbankWrap.set_normal_mode() == TbkReturn.TBK_OK;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to set normal mode", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public bool Polling()
        {
            if (_configured)
            {
                try
                {
                    return TransbankWrap.polling() == TbkReturn.TBK_OK;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to locate POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public RegisterCloseResponse RegisterClose()
        {
            if (_configured)
            {
                try
                {
                    RegisterCloseResponse response = new RegisterCloseResponse(TransbankWrap.register_close());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankRegisterCloseException("Register Close retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute register close in pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public LoadKeysResponse LoadKeys()
        {
            if (_configured)
            {
                try
                {
                    LoadKeysResponse response = new LoadKeysResponse(TransbankWrap.load_keys());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankLoadKeysException("Load Keys retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to load Keys in pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public bool ClosePort()
        {
            if (_configured)
            {
                try
                {
                    if (TransbankWrap.close_port() == TbkReturn.TBK_OK)
                    {
                        _configured = false;
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to close port: " + Port, e);
                }
            }
            else
            {
                return true;
            }
        }
    }
}
