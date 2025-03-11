using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services
{
	public class ProductService : IProductService
	{
		TestDbContext _ctx;

		public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public ProductList  ListProducts(int page)
		{
			var products = _ctx.Products.Skip((page - 1) * 10).Take(10).ToList();

            return new ProductList() {  HasNext=false, TotalCount = products.Count, Products = products };
		}

	}
}
