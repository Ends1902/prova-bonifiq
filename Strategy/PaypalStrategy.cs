namespace ProvaPub.Strategy
{
    public class PaypalStrategy : PaymentStrategy
    {
        public void DoPayment(decimal paymentValue)
        {
            // realiza pagamento via paypal
            Console.WriteLine("Pagamento realizado via paypal");
        }
    }
}
