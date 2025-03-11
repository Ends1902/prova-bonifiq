using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using System.Linq.Expressions;

namespace ProvaPub.UnitTests
{
    public class PurchaseTests
    {


        private readonly Mock<TestDbContext> _context;
        private readonly CustomerService _customerService;
        private readonly Mock<ICustomerService> _customerServiceMock;

        public PurchaseTests()
        {

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
                .Options;

            _context = new Mock<TestDbContext>(options);

            _customerService = new CustomerService(_context.Object);
            _customerServiceMock = new Mock<ICustomerService>();
        }




        [Fact]
        public async Task GivenNegativeCustomerId_WhenCallPurchase_ThenShouldThrowException()
        {
            //Arrange
            int customerId = -1;
            decimal purchaseAmount = 100M;

            //Act
            Func<Task> returnOfPurchaseProccess = async () => await _customerService.CanPurchase(customerId, purchaseAmount);


            //Assert
            await returnOfPurchaseProccess.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GivenNegativePurchaseValue_WhenCallPurchase_ThenShouldThrowException()
        {
            //Arrange
            int customerId = 1;
            decimal purchaseAmount = -100M;

            //Act
            Func<Task> returnOfPurchaseProccess = async () => await _customerService.CanPurchase(customerId, purchaseAmount);


            //Assert
            await returnOfPurchaseProccess.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GivenNonRegisteredCustomer_WhenCallPurchase_ShouldThrowException()
        {
            //Arrange
            int customerId = 1;
            decimal purchaseAmount = 100M;

            Mock<DbSet<Customer>> _mockDbSet = new Mock<DbSet<Customer>>();

            _context.Setup(x => x.Customers).Returns(_mockDbSet.Object);

            //Act
            Func<Task> returnOfPurchaseProccess = async () => await _customerService.CanPurchase(customerId, purchaseAmount);

            //Assert
            await returnOfPurchaseProccess
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"Customer Id {customerId} does not exists");
        }

        [Fact]
        public async Task GivenCustomerTryingPurchaseMoreThanOncePerMonth_WhenCallPurchase_ShouldReturnFalse()
        {
            //Arrange
            int customerId = 1;
            decimal purchaseAmount = 100M;


            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Ednaldo" },
                new Customer { Id = 2, Name = "Santos" }
            }.AsQueryable();

            var _mockDbSet = new Mock<DbSet<Customer>>();

            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            var orders = new List<Order>
            {
                new Order { Id = 1, Customer = customers.First(), CustomerId = customerId},
                new Order { Id = 2, Customer = customers.First(), CustomerId = customerId }
            }.AsQueryable();

            var _mockDbSetOrder = new Mock<DbSet<Order>>();

            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            _context.Setup(x => x.Customers).Returns(_mockDbSet.Object);
            _context.Setup(x => x.Orders).Returns(_mockDbSetOrder.Object);

            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                  .ReturnsAsync((object[] ids) => customers.FirstOrDefault(c => c.Id == (int)ids[0]));

            _customerServiceMock.Setup(x => x.HasMoreThanOnePurchaseInAmonth(customerId)).ReturnsAsync(true);

            //Act
            bool canPurchase = await _customerServiceMock.Object.CanPurchase(customerId, purchaseAmount);

            //Assert
            canPurchase.Should().BeFalse();
        }

        [Fact]
        public async Task GivenCustomerTryingPurchaseMoreForTheFirstTimeOverHundred_WhenCallPurchase_ShouldReturnFalse()
        {
            //Arrange
            int customerId = 1;
            decimal purchaseAmount = 100M;


            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Ednaldo" },
                new Customer { Id = 2, Name = "Santos" }
            }.AsQueryable();

            var _mockDbSet = new Mock<DbSet<Customer>>();

            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            var orders = new List<Order>
            {
                new Order { Id = 1, Customer = customers.First(), CustomerId = customerId},
                new Order { Id = 2, Customer = customers.First(), CustomerId = customerId }
            }.AsQueryable();

            var _mockDbSetOrder = new Mock<DbSet<Order>>();

            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            _context.Setup(x => x.Customers).Returns(_mockDbSet.Object);
            _context.Setup(x => x.Orders).Returns(_mockDbSetOrder.Object);

            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                  .ReturnsAsync((object[] ids) => customers.FirstOrDefault(c => c.Id == (int)ids[0]));

            _customerServiceMock.Setup(x => x.FirstPurchaseIsMaximumHundred(customerId, purchaseAmount)).ReturnsAsync(false);

            //Act
            bool canPurchase = await _customerServiceMock.Object.CanPurchase(customerId, purchaseAmount);

            //Assert
            canPurchase.Should().BeFalse();
        }

        [Fact]
        public async Task GivenCustomerTryingPurchaseOutOfElegibleTime_WhenCallPurchase_ShouldReturnFalse()
        {
            //Arrange
            int customerId = 1;
            decimal purchaseAmount = 100M;


            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Ednaldo" },
                new Customer { Id = 2, Name = "Santos" }
            }.AsQueryable();

            var _mockDbSet = new Mock<DbSet<Customer>>();

            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            _mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            var orders = new List<Order>
            {
                new Order { Id = 1, Customer = customers.First(), CustomerId = customerId},
                new Order { Id = 2, Customer = customers.First(), CustomerId = customerId }
            }.AsQueryable();

            var _mockDbSetOrder = new Mock<DbSet<Order>>();

            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            _mockDbSetOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            _context.Setup(x => x.Customers).Returns(_mockDbSet.Object);
            _context.Setup(x => x.Orders).Returns(_mockDbSetOrder.Object);

            _mockDbSet.Setup(m => m.FindAsync(It.IsAny<int>()))
                  .ReturnsAsync((object[] ids) => customers.FirstOrDefault(c => c.Id == (int)ids[0]));

            _customerServiceMock.Setup(x => x.IsElegibleTimeToPurchase()).ReturnsAsync(false);

            //Act
            bool canPurchase = await _customerServiceMock.Object.CanPurchase(customerId, purchaseAmount);

            //Assert
            canPurchase.Should().BeFalse();
        }

    }
}