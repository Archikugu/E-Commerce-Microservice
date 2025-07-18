﻿namespace MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;


public class ResultProductDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
}
