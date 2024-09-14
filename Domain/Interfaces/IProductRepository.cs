using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository
{
    Task<Product> AddProductAsync(Product product);
    Task<Product?> GetProductAsync(int id);
    Task<IEnumerable<Product?>?> GetAllProductsAsync();
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
}

