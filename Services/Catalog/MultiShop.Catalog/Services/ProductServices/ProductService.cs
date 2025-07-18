﻿using AutoMapper;
using MongoDB.Driver;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Entities;
using MultiShop.Catalog.Settings;

namespace MultiShop.Catalog.Services.ProductServices;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IMongoCollection<Product> _productsCollection;
    private readonly IMongoCollection<Category> _categoriesCollection;
    public ProductService(IMapper mapper, IDatabaseSettings _databaseSettings)
    {
        var client = new MongoClient(_databaseSettings.ConnectionString);
        var database = client.GetDatabase(_databaseSettings.DatabaseName);
        _productsCollection = database.GetCollection<Product>(_databaseSettings.ProductCollectionName);
        _categoriesCollection = database.GetCollection<Category>(_databaseSettings.CategoryCollectionName);
        _mapper = mapper;           
    }

    public async Task CreateProductAsync(CreateProductDto createProductDto)
    {
        var values = _mapper.Map<Product>(createProductDto);
        await _productsCollection.InsertOneAsync(values);
    }

    public async Task DeleteProductAsync(string id)
    {
        await _productsCollection.DeleteOneAsync(x => x.ProductId == id);
    }

    public async Task<List<ResultProductDto>> GetAllProductAsync()
    {
        var values = await _productsCollection.Find(x => true).ToListAsync();
        return _mapper.Map<List<ResultProductDto>>(values);
    }

    public async Task<GetByIdProductDto> GetByIdProductAsync(string id)
    {
        var values = await _productsCollection.Find<Product>(x => x.ProductId == id).FirstOrDefaultAsync();
        return _mapper.Map<GetByIdProductDto>(values);
    }

    public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryAsync()
    {
        var values = await _productsCollection.Find(x => true).ToListAsync();
        foreach (var product in values)
        {
            product.Category = await _categoriesCollection.Find(x => x.CategoryId == product.CategoryId).FirstAsync();
        }
        return _mapper.Map<List<ResultProductsWithCategoryDto>>(values);
    }

    public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
    {
        var values = _mapper.Map<Product>(updateProductDto);
        await _productsCollection.FindOneAndReplaceAsync(x => x.ProductId == updateProductDto.ProductId, values);
    }
}