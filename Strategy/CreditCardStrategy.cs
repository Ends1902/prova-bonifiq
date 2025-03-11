namespace ProvaPub.Strategy
{
    public class CreditCardStrategy : PaymentStrategy
    {
        public void DoPayment(decimal paymentValue)
        {
            // realiza pagamento via cartão de crédito
            Console.WriteLine("Pagamento realizado via cartão de crédito");
        }
    }
}
