using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services
{
    public class CustomerService : ICustomerService
    {
        TestDbContext _ctx;

        public CustomerService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public CustomerList ListCustomers(int page)
        {
            var customers = _ctx.Customers.Skip((page - 1) * 10).Take(10).ToList();

            return new CustomerList() { HasNext = false, TotalCount = customers.Count, Customers = customers };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));


            if (await IsNonRegistered(customerId)) return false;

            if(await HasMoreThanOnePurchaseInAmonth(customerId)) return false;

            if (await FirstPurchaseIsMaximumHundred(customerId, purchaseValue)) return false;

            if (!await IsElegibleTimeToPurchase()) return false;


            return true;
        }

        public async Task<bool> IsNonRegistered(int customerId)
        {
            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");
            return false;
        }

        public async Task<bool> HasMoreThanOnePurchaseInAmonth(int customerId)
        {
            //Business Rule: A customer can purchase only a single time per month
            var baseDate = DateTime.UtcNow.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return true;
            
            return false;
        }

        public async Task<bool> FirstPurchaseIsMaximumHundred(int customerId, decimal purchaseValue)
        {
            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return true;

            return false;
        }

        public async Task<bool> IsElegibleTimeToPurchase()
        {
            //Business Rule: A customer can purchases only during business hours and working days
            if (DateTime.UtcNow.Hour < 8 || DateTime.UtcNow.Hour > 18 || DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }
    }
}
