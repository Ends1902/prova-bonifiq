using ProvaPub.Models;
using System.Transactions;

namespace ProvaPub.Services.Interfaces
{
    public interface IProductService 
    {
        ProductList ListProducts(int page);
    }
}
