using Transbank.POS.Utils;
using Transbank.POS.Responses;
using Transbank.POS.Exceptions;
using System;
using System.Collections.Generic;

namespace Transbank.POS
{
    public class POS
    {
        private static readonly POS _instance = new POS();
        private bool _configured = false;
        private TbkBaudrate _defaultBaudrate = TbkBaudrate.TBK_115200;

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

        public void OpenPort(string portName) => OpenPort(portName, _defaultBaudrate);

        public void OpenPort(string portName, TbkBaudrate baudrate)
        {
            try
            {
                if (TransbankWrap.open_port(portName, (int)baudrate) == TbkReturn.TBK_OK)
                    _configured = true;
                else
                    throw new TransbankException("Unable to Open selected port: " + portName);
            }
            catch (TransbankException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to Open selected port: " + portName, e);
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
                        throw new TransbankSaleException("Sale returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankSaleException)
                {
                    throw;
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

        public LastSaleResponse LastSale()
        {
            if (_configured)
            {
                try
                {
                    LastSaleResponse response = new LastSaleResponse(TransbankWrap.last_sale());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankLastSaleException("LastSale returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankLastSaleException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute LastSale on pos", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public RefundResp Refund(int operationID)
        {
            if (_configured)
            {
                try
                {
                    Responses.RefundResp response = new Responses.RefundResp(TransbankWrap.refund(operationID));
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankRefundException("Refund returned an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankRefundException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to make Refund on POS", e);
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

        public bool Poll()
        {
            if (_configured)
            {
                try
                {
                    return TransbankWrap.poll() == TbkReturn.TBK_OK;
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

        public CloseResponse Close()
        {
            if (_configured)
            {
                try
                {
                    CloseResponse response = new CloseResponse(TransbankWrap.close());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankCloseException("Close retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankCloseException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to execute close in pos", e);
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
                catch (TransbankLoadKeysException)
                {
                    throw;
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

        public GetTotalsResponse GetTotals()
        {
            if (_configured)
            {
                try
                {
                    GetTotalsResponse response = new GetTotalsResponse(TransbankWrap.get_totals());
                    if (response.Success)
                    {
                        return response;
                    }
                    else
                    {
                        throw new TransbankGetTotalsException("Get Totals retured an error: " + response.ResponseMessage, response);
                    }
                }
                catch (TransbankGetTotalsException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to get totals in POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }

        public List<DetailResponse> Details(int op)
        {
            if (_configured)
            {
                try
                {
                    string[] lines = TransbankWrap.sales_detail(op).Split('\n');
                    var details = new List<DetailResponse>();

                    foreach (string line in lines)
                    {
                        details.Add(new DetailResponse(line));
                    }

                    return details;
                }
                catch (TransbankSalesDetailException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new TransbankException("Unable to get details in POS", e);
                }
            }
            else
            {
                throw new TransbankException("Port not Configured");
            }
        }
    }
}
