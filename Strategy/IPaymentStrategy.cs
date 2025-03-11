namespace ProvaPub.Strategy
{
    public interface IPaymentStrategy
    {
        public void DoPayment(string paymentType, decimal amount);
       
       
    }
}

