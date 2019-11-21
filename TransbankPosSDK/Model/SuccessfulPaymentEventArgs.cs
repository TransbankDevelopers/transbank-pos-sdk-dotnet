using System;
using Transbank.Onepay.Model;

namespace Transbank.POS.Model
{
    public class SuccessfulPaymentEventArgs : EventArgs
    {
        public SuccessfulPaymentEventArgs(TransactionCommitResponse response, string externalUniqueNumber)
        { 
            Transaction = response;
            _externalUniqueNumber = externalUniqueNumber;
        }
        private readonly TransactionCommitResponse Transaction;
        private readonly string _externalUniqueNumber;

        public long Amount { get => Transaction.Amount; }
        public string ExternalUniqueNumber { get => _externalUniqueNumber; }
        public string AuthorizationCode { get => Transaction.AuthorizationCode; }
        public string BuyOrder { get => Transaction.BuyOrder; }
        public long InstallmentsAmount { get => Transaction.InstallmentsAmount; }
        public long InstallmentsNumber { get => Transaction.InstallmentsNumber; }
        public long IssuedAt { get => Transaction.IssuedAt; }
        public string Occ { get => Transaction.Occ; }
        public string Signature { get => Transaction.Signature; }
        public string TransactionDesc { get => Transaction.TransactionDesc; }

        public override string ToString()
        {
            return "Amount: " + Amount + "\n" +
                    "AuthorizationCode: " + AuthorizationCode + "\n" +
                    "BuyOrder: " + BuyOrder + "\n" +
                    "InstallmentsAmount: " + InstallmentsAmount + "\n" +
                    "InstallmentsNumber: " + InstallmentsNumber + "\n" +
                    "IssuedAt: " + IssuedAt + "\n" +
                    "Occ: " + Occ + "\n" +
                    "Signature: " + Signature + "\n" +
                    "TransactionDesc: " + TransactionDesc + "\n";

        }
    }
}
