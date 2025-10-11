using MongoDB.Driver;
using MultiShop.Catalog.Entities;
using MultiShop.Catalog.Settings;
using System.Threading.Tasks;
using MultiShop.Catalog.Dtos.ProductDtos;
using AutoMapper;

namespace MultiShop.Catalog.Services.StatisticServices;

public class StatisticService : IStatisticService
{
    private readonly IMongoCollection<Category> _categoriesCollection;
    private readonly IMongoCollection<Product> _productsCollection;
    private readonly IMongoCollection<Brand> _brandsCollection;
    private readonly IMapper _mapper;

    public StatisticService(IDatabaseSettings databaseSettings)
    {
        var client = new MongoClient(databaseSettings.ConnectionString);
        var database = client.GetDatabase(databaseSettings.DatabaseName);
        _categoriesCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
        _productsCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
        _brandsCollection = database.GetCollection<Brand>(databaseSettings.BrandCollectionName);
    }

    public async Task<long> GetCategoryCountAsync()
    {
        return await _categoriesCollection.CountDocumentsAsync(x => true);
    }

    public async Task<long> GetProductCountAsync()
    {
        return await _productsCollection.CountDocumentsAsync(x => true);
    }

    public async Task<long> GetBrandCountAsync()
    {
        return await _brandsCollection.CountDocumentsAsync(x => true);
    }

    public async Task<decimal> GetAverageProductPriceAsync()
    {
        var products = await _productsCollection.Find(x => true).Project(p => p.Price).ToListAsync();
        if (products == null || products.Count == 0)
        {
            return 0m;
        }
        decimal sum = 0m;
        foreach (var price in products)
        {
            sum += price;
        }
        return sum / products.Count;
    }

    public async Task<ResultProductDto> GetCheapestProductAsync()
    {
        var product = await _productsCollection
            .Find(x => true)
            .SortBy(p => p.Price)
            .FirstOrDefaultAsync();
        if (product == null)
        {
            return null;
        }
        // Manual mapping to avoid requiring IMapper injection here
        return new ResultProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            CategoryId = product.CategoryId
        };
    }

    public async Task<ResultProductDto> GetMostExpensiveProductAsync()
    {
        var product = await _productsCollection
            .Find(x => true)
            .SortByDescending(p => p.Price)
            .FirstOrDefaultAsync();
        if (product == null)
        {
            return null;
        }
        return new ResultProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            CategoryId = product.CategoryId
        };
    }
}


