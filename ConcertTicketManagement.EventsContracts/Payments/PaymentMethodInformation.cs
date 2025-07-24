
namespace ConcertTicketManagement.Contracts.Payments
{
    public class PaymentMethodInformation
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpirationDate { get; set; }
        public string Cvv { get; set; }
        public string BillingAddress { get; set; }

        public PaymentMethodInformation(
            string cardNumber,
            string cardHolderName,
            string expirationDate,
            string cvv,
            string billingAddress)
        {
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            ExpirationDate = expirationDate;
            Cvv = cvv;
            BillingAddress = billingAddress;
        }
    }
}
