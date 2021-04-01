using System;
using Transbank.POS.Responses.IntegradoResponses;
using Transbank.POS.Exceptions.IntegradoExceptions;
using Transbank.POS.Utils;
using System.Collections.Generic;
using Transbank.POS.Responses.CommonResponses;
using Transbank.POS.Exceptions.CommonExceptions;

namespace Transbank.POSIntegrado
{
    public class POSIntegrado : Serial
    {
        private POSIntegrado()
        {
            
        }

        public static POSIntegrado Instance { get; } = new POSIntegrado();
        
        [ObsoleteAttribute("This method is obsolete. Call SaleResponse(int, string) instead.", false)]
        public SaleResponse Sale(int amount, int ticket)
        {
            return Sale(amount, ticket.ToString());
        }

        public SaleResponse Sale(int amount, string ticket, bool sendStatus = false)
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
                WriteData(MessageWithLRC(message), intermediateMessages: sendStatus).Wait();
                return new SaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankSaleException($"Unable to execute sale on pos", e);
            }
        }

        public MultiCodeSaleResponse MultiCodeSale(int amount, string ticket, long commerceCode = 0, bool sendStatus = false)
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
                WriteData(MessageWithLRC(message), intermediateMessages: sendStatus).Wait();
                return new MultiCodeSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeSaleException($"Unable to execute multicode sale on pos", e);
            }
        }

        public LastSaleResponse LastSale()
        {
            try
            {
                WriteData("0250|x").Wait();
                return new LastSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankLastSaleException($"Unable to recover last sale from pos", e);
            }
        }

        public MultiCodeLastSaleResponse MultiCodeLastSale(bool getVoucherInfo)
        {
            try
            {
                string message = $"0280|{Convert.ToInt32(getVoucherInfo)}";
                WriteData(MessageWithLRC(message)).Wait();
                return new MultiCodeLastSaleResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankMultiCodeLastSaleException($"Unable to recover last sale from pos", e);
            }
        }


        public RefundResponse Refund(int operationID)
        {
            string message = $"1200|{operationID}|";

            try
            {
                WriteData(MessageWithLRC(message)).Wait();
                return new RefundResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankRefundException("Unable to make Refund on POS", e);
            }
        }

        public TotalsResponse Totals()
        {
            try
            {
                WriteData("0700||").Wait();
                return new TotalsResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankTotalsException("Unable to get totals from POS", e);
            }
        }

        public List<DetailResponse> Details(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<DetailResponse> details = new List<DetailResponse>();
            try
            {
                WriteData(MessageWithLRC(message), printOnPOS: printOnPOS, saleDetail: true).Wait();

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

        public List<MultiCodeDetailResponse> MultiCodeDetails(bool printOnPOS = true)
        {
            string message = $"0260|{Convert.ToInt32(!printOnPOS)}|";
            List<MultiCodeDetailResponse> details = new List<MultiCodeDetailResponse>();
            try
            {
                WriteData(MessageWithLRC(message), printOnPOS: printOnPOS, saleDetail: true).Wait();

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

        public CloseResponse Close()
        {
            try
            {
                WriteData("0500||").Wait();
                return new CloseResponse(CurrentResponse);
            }
            catch (Exception e)
            {
                throw new TransbankCloseException("Unable to execute close in pos", e);
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

        public bool Poll()
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
                Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Poll command on port {Port.PortName}", e);
            }
        }

        public bool SetNormalMode()
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
                Port.BaseStream.ReadAsync(buffer, 0, 1).Wait();
                return buffer[0] == ACK;
            }
            catch (Exception e)
            {
                throw new TransbankException($"Unable to send Normal Mode command on port {Port.PortName}", e);
            }
        }
    }
}
