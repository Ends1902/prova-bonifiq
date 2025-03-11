namespace ProvaPub.Strategy
{
    public class PaymentStrategy : IPaymentStrategy
    {
        public void DoPayment(string paymentType, decimal amount)
        {
            {
                if (paymentType == "pix")
                    new PixStrategy().DoPayment(amount);

                if (paymentType == "creditCard")
                    new CreditCardStrategy().DoPayment(amount);

                if (paymentType == "paypal")
                    new PaypalStrategy().DoPayment(amount);
            }
        }
    }
}
