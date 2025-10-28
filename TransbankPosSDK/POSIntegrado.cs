using System;
using Transbank.Responses.IntegradoResponses;
using Transbank.Exceptions.IntegradoExceptions;
using System.Collections.Generic;
using Transbank.Responses.CommonResponses;
using Transbank.Exceptions.CommonExceptions;
using System.Threading.Tasks;
using Transbank.SerialPortHandler;
using Transbank.Services;

namespace Transbank.POSIntegrado
{
    public class POSIntegrado
    {
        public event EventHandler<IntermediateResponse> IntermediateResponseChange;
        private ISerialHandler _handler;
        private PosService _posService;
        private POSIntegrado()
        {
            _handler = new SerialHandler();
            _posService = new PosService(_handler);
            _posService.IntermediateResponseReceived += OnIntermediateResponseReceived;
        }

        internal POSIntegrado(ISerialHandler handler, PosService service)
        {
            _handler = handler;
            _posService = service;
            _posService.IntermediateResponseReceived += OnIntermediateResponseReceived;
        }

        private void OnIntermediateResponseReceived(object sender, string response)
        {
            IntermediateResponseChange?.Invoke(this, new IntermediateResponse(response));
        }

        public static POSIntegrado Instance { get; } = new POSIntegrado();

        public List<string> ListPorts()
        {
            return _posService.getPorts();
        }

        public void OpenPort(string portName, int baudrate = 115200)
        {
            _posService.OpenPort(portName, baudrate);
        }

        public void ClosePort()
        {
            _posService.ClosePort();
        }

        public bool IsPortOpen => _posService.IsPortOpen;
        
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
            string voucherFlag = sendVoucher ? "1" : "0";
            string statusFlag = sendStatus ? "1" : "0";
            string command = $"0200|{amount}|{ticket}||{voucherFlag}|{statusFlag}|";
            try
            {
                string response = await _posService.ProcessNormalCommand(command);
                return new SaleResponse(response);
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
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            string code = commerceCode != 0 ? commerceCode.ToString() : "";
            string voucherFlag = sendVoucher ? "1" : "0";
            string statusFlag = sendStatus ? "1" : "0";
            string command = $"0270|{amount}|{ticket}||{voucherFlag}|{statusFlag}|{code}|";
            try
            {
                string response = await _posService.ProcessNormalCommand(command);
                return new MultiCodeSaleResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeSaleException($"Unable to execute multicode sale on pos", e);
            }
        }

        public async Task<LastSaleResponse> LastSale()
        {
            try
            {
                string command = "0250|";
                string response = await _posService.ProcessNormalCommand(command);
                return new LastSaleResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankLastSaleException($"Unable to recover last sale from pos", e);
            }
        }

        public async Task<MultiCodeLastSaleResponse> MultiCodeLastSale(bool getVoucherInfo)
        {
            try
            {
                string command = $"0280|{Convert.ToInt32(getVoucherInfo)}";
                string response = await _posService.ProcessNormalCommand(command);
                return new MultiCodeLastSaleResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeLastSaleException($"Unable to recover last sale from pos", e);
            }
        }


        public async Task<RefundResponse> Refund(int operationID)
        {

            try
            {
                string command = $"1200|{operationID}|";
                string response = await _posService.ProcessNormalCommand(command);
                return new RefundResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankRefundException("Unable to make Refund on POS", e);
            }
        }

        public async Task<TotalsResponse> Totals()
        {
            try
            {
                string command = "0700|";
                string response = await _posService.ProcessNormalCommand(command);
                return new TotalsResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankTotalsException("Unable to get totals from POS", e);
            }
        }

        public async Task<List<DetailResponse>> Details(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<DetailResponse> details = new List<DetailResponse>();
            try
            {
                List<string> responses = await _posService.ProcessDetailsCommand(message, printOnPOS);

                foreach (string sale in responses)
                {
                    details.Add(new DetailResponse(sale));
                }
                return details;
            }
            catch (Exception e)
            {
                throw new TransbankSalesDetailException("Unabel to request sale detail on pos", e);
            }
        }

        public async Task<List<MultiCodeDetailResponse>> MultiCodeDetails(bool printOnPOS = true)
        {
            string command = $"0290|{Convert.ToInt32(!printOnPOS)}|";
            List<MultiCodeDetailResponse> details = new List<MultiCodeDetailResponse>();
            try
            {
                List<string> responses = await _posService.ProcessDetailsCommand(command, printOnPOS);

                foreach (string sale in responses)
                {
                    details.Add(new MultiCodeDetailResponse(sale));
                }
                return details;
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeDetailException("Unabel to request sale detail on pos", e);
            }
        }

        public async Task<CloseResponse> Close()
        {
            try
            {
                string command = "0500||";
                string response = await _posService.ProcessNormalCommand(command);
                return new CloseResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankCloseException("Unable to execute close in pos", e);
            }
        }

        public async Task<LoadKeysResponse> LoadKeys()
        {
            try
            {
                string command = "0800";
                string response = await _posService.ProcessNormalCommand(command);
                return new LoadKeysResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankLoadKeysException("Unable to execute Load Keys in pos", e);
            }
        }

        public async Task<bool> Poll()
        {
            try
            {              
                string command = "0100";
                string response = await _posService.ProcessNormalCommand(command, shortResponse: true);
                return response == ((char)0x06).ToString();
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port", e);
            }
        }

        public async Task<bool> SetNormalMode()
        {
            try
            {
                string command = "0300";
                string response = await _posService.ProcessNormalCommand(command, shortResponse: true);
                return response == ((char)0x06).ToString();
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Normal Mode command on port", e);
            }
        }
    }
}
