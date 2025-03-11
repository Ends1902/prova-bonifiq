namespace ProvaPub.Strategy
{
    public class PixStrategy : PaymentStrategy
    {
        public void DoPayment(decimal paymentValue)
        {
            //Realiza pagamento via pix
            Console.WriteLine("Pagamento realizado via pix");
        }
    }
}
