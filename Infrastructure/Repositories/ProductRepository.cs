
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly List<Product>? _products;

    public ProductRepository()
    {
        // Initialize the in-memory product list
        _products = new List<Product>();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        // Find the product by ID
        var product = _products?.FirstOrDefault(p => p.Id == id);
        return await Task.FromResult(product);
    }

    public async Task<IEnumerable<Product?>?> GetAllProductsAsync()
    {
        // Return all products
        return await Task.FromResult(_products);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        // Set a unique ID for the new product (simulating auto-increment behavior)
        product.Id = _products?.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;

        // Add the product to the in-memory list
        _products?.Add(product);

        return await Task.FromResult(product);
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        var existingProduct = _products?.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct != null)
        {
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = _products?.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products?.Remove(product);
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }
}

