using Microsoft.AspNetCore.Mvc;
using MultiShop.RapidApiWebUI.Models;
using Newtonsoft.Json;

namespace MultiShop.RapidApiWebUI.Controllers
{
    public class CurrencyExchangeController : Controller
    {
        public async Task<IActionResult> Exchange()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://real-time-finance-data.p.rapidapi.com/currency-exchange-rate?from_symbol=USD&to_symbol=EUR&language=en"),
                Headers =
    {
        { "x-rapidapi-key", "5e9de8dc85msh8a73ebada00ada2p1e00cdjsne172b6e91af1" },
        { "x-rapidapi-host", "real-time-finance-data.p.rapidapi.com" },
    },

            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<ExchangeViewModel.Rootobject>(body);
                ViewBag.ExchangeRate = values.data.exchange_rate;
                ViewBag.PreviousClose = values.data.previous_close;
                Console.WriteLine(body);
                return View();
            }
        }
    }
}
