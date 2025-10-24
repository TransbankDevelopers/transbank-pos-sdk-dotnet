using System;
using System.Collections.Generic;
using Transbank.Exceptions.CommonExceptions;
using Transbank.Responses.CommonResponses;
using Transbank.Responses.AutoservicioResponse;
using Transbank.Exceptions.AutoservicioExceptions;
using System.Threading.Tasks;
using Transbank.SerialPortHandler;
using Transbank.Services;

namespace Transbank.POSAutoservicio
{
    public class POSAutoservicio
    {
        public event EventHandler<IntermediateResponse> IntermediateResponseChange;
        private ISerialHandler _handler;
        private PosService _posService;
        public POSAutoservicio()
        {
            _handler = new SerialHandler();
            _posService = new PosService(_handler, PosService.Model.AUTOSERVICIO);
            _posService.IntermediateResponseReceived += OnIntermediateResponseReceived;
        }

        internal POSAutoservicio(ISerialHandler handler, PosService service)
        {
            _handler = handler;
            _posService = service;
            _posService.IntermediateResponseReceived += OnIntermediateResponseReceived;
        }

        private void OnIntermediateResponseReceived(object sender, string response)
        {
            IntermediateResponseChange?.Invoke(this, new IntermediateResponse(response));
        }

        public static POSAutoservicio Instance { get; } = new POSAutoservicio();

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
        public async Task<bool> Poll()
        {
            try
            {              
                string command = "0100";
                string response = await _posService.SendNormalCommand(command, shortResponse: true);
                return response == ((char)0x06).ToString();
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port", e);
            }
        }

        public async Task<LoadKeysResponse> LoadKeys()
        {
            try
            {
                string command = "0800";
                string response = await _posService.SendNormalCommand(command);
                return new LoadKeysResponse(response);
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
                string command = "0070";
                string response = await _posService.SendNormalCommand(command, shortResponse: true);
                return response == ((char)0x06).ToString();
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Initialization command in pos", e);
            }
        }

        public async Task<InitializationResponse> InitializationResponse()
        {
            try
            {
                string command = "0080";
                string response = await _posService.SendNormalCommand(command);
                return new InitializationResponse(response);
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
            try
            {
                string command = $"0200|{amount}|{ticket}|{Convert.ToInt32(sendVoucher)}|{Convert.ToInt32(sendStatus)}";
                string response = await _posService.SendNormalCommand(command);
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
            if (ticket.Length > 20)
            {
                throw new TransbankMultiCodeSaleException("The ticket must be up to 20 characters.");
            }
            try
            {
                string code = commerceCode != 0 ? commerceCode.ToString() : "";
                string command = $"0270|{amount}|{ticket}|{Convert.ToInt32(sendVoucher)}|{Convert.ToInt32(sendStatus)}|{code}";
                string response = await _posService.SendNormalCommand(command);
                return new MultiCodeSaleResponse(response);
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
                string command = $"0250|{Convert.ToInt32(sendVoucher)}";
                string response = await _posService.SendNormalCommand(command);
                return new LastSaleResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankLastSaleException($"Unable to recover last sale from pos", e);
            }
        }

        public async Task<CloseResponse> Close(bool sendVoucher)
        {

            try
            {
                string command = $"0500|{Convert.ToInt32(sendVoucher)}";
                string response = await _posService.SendNormalCommand(command);
                return new CloseResponse(response);
            }
            catch (Exception e)
            {
                throw new TransbankCloseException("Unable to execute close in pos", e);
            }
        }
    }
}
