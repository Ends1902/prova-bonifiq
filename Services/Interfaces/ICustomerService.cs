namespace ProvaPub.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<bool> CanPurchase(int customerId, decimal purchaseValue);

        Task<bool> IsNonRegistered(int customerId);

        Task<bool> HasMoreThanOnePurchaseInAmonth(int customerId);

        Task<bool> FirstPurchaseIsMaximumHundred(int customerId, decimal purchaseValue);

        Task<bool> IsElegibleTimeToPurchase();

    }
}
