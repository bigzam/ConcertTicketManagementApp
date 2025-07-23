
namespace ConcertTicketManagement.Contracts.Payments
{
    public class PaymentMethod
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Cvv { get; set; }
        public string BillingAddress { get; set; }

        public PaymentMethod(
            string cardNumber,
            string cardHolderName,
            DateTime expirationDate,
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
