using MultiShop.WebUI.Dtos.BasketDtos;

namespace MultiShop.WebUI.Services.BasketServices;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;
    public BasketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BasketTotalDto> GetBasket()
    {
        var resp = await _httpClient.GetAsync("baskets");
        if (resp.IsSuccessStatusCode)
        {
            var data = await resp.Content.ReadFromJsonAsync<BasketTotalDto>();
            return data ?? new BasketTotalDto { BasketItems = new List<BasketItemDto>() };
        }
        if ((int)resp.StatusCode == 404)
        {
            return new BasketTotalDto { BasketItems = new List<BasketItemDto>() };
        }
        var error = await resp.Content.ReadAsStringAsync();
        throw new Exception($"Failed to retrieve basket. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
    }

    public async Task SaveBasket(BasketTotalDto basketTotalDto)
    {
        basketTotalDto ??= new BasketTotalDto();
        basketTotalDto.DiscountCode ??= string.Empty;
        basketTotalDto.BasketItems ??= new List<BasketItemDto>();
        // UserId sunucuda LoginService ile set ediliyor; istemci göndermese de sorun olmamalı

        var resp = await _httpClient.PostAsJsonAsync("baskets", basketTotalDto);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to save basket. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }

    public async Task DeleteBasket(string userId)
    {
        var resp = await _httpClient.DeleteAsync($"baskets/{userId}");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete basket. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }

    public async Task AddBasketItem(BasketItemDto basketItemDto)
    {
        var values = await GetBasket();
        values.BasketItems ??= new List<BasketItemDto>();
        var existingItem = values.BasketItems.FirstOrDefault(p => p.ProductId == basketItemDto.ProductId);
        if (existingItem == null)
        {
            values.BasketItems.Add(basketItemDto);
        }
        else
        {
            existingItem.Quantity += basketItemDto.Quantity;
        }

        await SaveBasket(values);
    }

    public async Task<bool> RemoveBasketItem(string productId)
    {
        var values = await GetBasket();
        if (values != null && values.BasketItems.Any())
        {
            var deleteItem = values.BasketItems.FirstOrDefault(p => p.ProductId == productId);
            if (deleteItem != null)
            {
                values.BasketItems.Remove(deleteItem);
                await SaveBasket(values);
                return true;
            }
        }
        return false;
    }
}
