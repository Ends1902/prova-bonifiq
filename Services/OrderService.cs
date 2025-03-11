using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Strategy;
using System.Text.Json;

namespace ProvaPub.Services
{
    public class OrderService
    {
        TestDbContext _ctx;

        public OrderService(TestDbContext ctx)
        {
            _ctx = ctx;
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = true 
            };
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
        {

            new PaymentStrategy().DoPayment(paymentMethod, paymentValue);

            var insertedEntity = await InsertOrder(new Order
            {
                OrderDate = DateTime.UtcNow,
                CustomerId = customerId,
                Customer = new Customer { Name = "Ednaldo Santos" }
            });


            TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

            // Converte de UTC para horário do Brasil
            DateTime brazilDate = TimeZoneInfo.ConvertTimeFromUtc(insertedEntity.OrderDate, brazilTimeZone);

            insertedEntity.OrderDate = brazilDate;

            return insertedEntity;
        }

        public async Task<Order> InsertOrder(Order order)
        {
            //Insere pedido no banco de dados
            return (await _ctx.Orders.AddAsync(order)).Entity;
        }
    }
}
