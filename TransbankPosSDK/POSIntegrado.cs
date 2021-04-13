using System;
using Transbank.Responses.IntegradoResponses;
using Transbank.Exceptions.IntegradoExceptions;
using Transbank.Utils;
using System.Collections.Generic;
using Transbank.Responses.CommonResponses;
using Transbank.Exceptions.CommonExceptions;
using System.Threading.Tasks;

namespace Transbank.POSIntegrado
{
    public class POSIntegrado : Serial
    {
        private POSIntegrado()
        {
            
        }

        public static POSIntegrado Instance { get; } = new POSIntegrado();
        
        public async Task<SaleResponse> Sale(int amount, string ticket, bool sendStatus = false)
        {
            if (amount <= 0)
            {
                throw new TransbankSaleException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            string message = $"0200|{amount}|{ticket}|||{Convert.ToInt32(sendStatus)}|";
            try
            {
                await WriteData(MessageWithLRC(message), intermediateMessages: sendStatus);
                return new SaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankSaleException($"Unable to execute sale on pos", e);
            }
        }

        public async Task<MultiCodeSaleResponse> MultiCodeSale(int amount, string ticket, long commerceCode = 0, bool sendStatus = false)
        {
            if (amount <= 0)
            {
                throw new TransbankSaleException("Amount must be greater than 0");
            }
            if (ticket.Length != 6)
            {
                throw new TransbankSaleException("Ticket must be 6 characters.");
            }
            string code = commerceCode != 0 ? commerceCode.ToString() : "";
            string message = $"0270|{amount}|{ticket}|| |{Convert.ToInt32(sendStatus)}|{code}|";
            try
            {
                await WriteData(MessageWithLRC(message), intermediateMessages: sendStatus);
                return new MultiCodeSaleResponse(CurrentResponse);
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
                await WriteData("0250|x");
                return new LastSaleResponse(CurrentResponse);
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
                string message = $"0280|{Convert.ToInt32(getVoucherInfo)}";
                await WriteData(MessageWithLRC(message));
                return new MultiCodeLastSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeLastSaleException($"Unable to recover last sale from pos", e);
            }
        }


        public async Task<RefundResponse> Refund(int operationID)
        {
            string message = $"1200|{operationID}|";

            try
            {
                await WriteData(MessageWithLRC(message));
                return new RefundResponse(CurrentResponse);
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
                await WriteData("0700||");
                return new TotalsResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankTotalsException("Unable to get totals from POS", e);
            }
        }

        public async Task<List<DetailResponse>> Details(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<DetailResponse> details = new List<DetailResponse>();
            try
            {
                await WriteData(MessageWithLRC(message), printOnPOS: printOnPOS, saleDetail: true);

                foreach (string sale in SaleDetail)
                {
                    details.Add(new DetailResponse(sale));
                }
            }
            catch (Exception e)
            {
                throw new TransbankSalesDetailException("Unabel to request sale detail on pos", e);
            }
            return details;
        }

        public async Task<List<MultiCodeDetailResponse>> MultiCodeDetails(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<MultiCodeDetailResponse> details = new List<MultiCodeDetailResponse>();
            try
            {
                await WriteData(MessageWithLRC(message), printOnPOS: printOnPOS, saleDetail: true);

                foreach (string sale in SaleDetail)
                {
                    details.Add(new MultiCodeDetailResponse(sale));
                }
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeDetailException("Unabel to request sale detail on pos", e);
            }
            return details;
        }

        public async Task<CloseResponse> Close()
        {
            try
            {
                await WriteData("0500||");
                return new CloseResponse(CurrentResponse);
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
                await WriteData("0800");
                return new LoadKeysResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLoadKeysException("Unable to execute Load Keys in pos", e);
            }
        }

        public async Task<bool> Poll()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (CantWrite())
            {
                throw new TransbankException($"Unable to Poll port {Port.PortName} is closed");
            }
            try
            {              
                byte[] buffer = new byte[1];
                Port.Write("0100");
                await Port.BaseStream.ReadAsync(buffer, 0, 1);
                return CheckACK(buffer[0]);
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

        public async Task<bool> SetNormalMode()
        {
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            if (CantWrite())
            {
                throw new TransbankException($"Unable to Set Normal Mode port {Port.PortName} is closed");
            }
            try
            {
                byte[] buffer = new byte[1];
                Port.Write("0300\0");
                await Port.BaseStream.ReadAsync(buffer, 0, 1);
                return CheckACK(buffer[0]);
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Normal Mode command on port {Port.PortName}", e);
            }
        }
    }
}
