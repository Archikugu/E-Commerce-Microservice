using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;
using MultiShop.WebUI.Services.StatisticsServices.UserStatistic;
using MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;
using MultiShop.WebUI.Services.MessageServices;
using MultiShop.WebUI.Services.StatisticsServices.DiscountStatisticServices;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.UserIdentityServices;
using System.Text.Json;
using System.Linq;
namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ICatalogStatisticsService _statisticsService;
        private readonly IUserStatisticsService _userStatisticsService;
        private readonly ICommentStatisticsService _commentStatisticsService;
        private readonly IMessageService _messageService;
        private readonly IDiscountStatisticsService _discountStatisticsService;
        private readonly IOrderOrderingService _orderService;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ICatalogStatisticsService statisticsService, 
            IUserStatisticsService userStatisticsService, 
            ICommentStatisticsService commentStatisticsService, 
            IMessageService messageService, 
            IDiscountStatisticsService discountStatisticsService,
            IOrderOrderingService orderService,
            IUserIdentityService userIdentityService,
            IHttpClientFactory httpClientFactory)
        {
            _statisticsService = statisticsService;
            _userStatisticsService = userStatisticsService;
            _commentStatisticsService = commentStatisticsService;
            _messageService = messageService;
            _discountStatisticsService = discountStatisticsService;
            _orderService = orderService;
            _userIdentityService = userIdentityService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var categoryCount = await _statisticsService.GetCategoryCountAsync();
            var productCount = await _statisticsService.GetProductCountAsync();
            var brandCount = await _statisticsService.GetBrandCountAsync();
            var averagePrice = await _statisticsService.GetAverageProductPriceAsync();
            var userCount = await _userStatisticsService.GetUserCountAsync();
            var cheapest = await _statisticsService.GetCheapestProductAsync();
            var mostExpensive = await _statisticsService.GetMostExpensiveProductAsync();
            var activeComments = await _commentStatisticsService.GetActiveCommentCountAsync();
            var passiveComments = await _commentStatisticsService.GetPassiveCommentCountAsync();
            var totalComments = await _commentStatisticsService.GetTotalCommentCountAsync();

            ViewBag.CategoryCount = categoryCount;
            ViewBag.ProductCount = productCount;
            ViewBag.BrandCount = brandCount;
            ViewBag.AverageProductPrice = averagePrice;
            ViewBag.CheapestProduct = cheapest;
            ViewBag.MostExpensiveProduct = mostExpensive;
            ViewBag.UserCount = userCount;
            ViewBag.ActiveCommentCount = activeComments;
            ViewBag.PassiveCommentCount = passiveComments;
            ViewBag.TotalCommentCount = totalComments;

            var recentMessages = await _messageService.GetLatestAsync(3);
            ViewBag.RecentMessages = recentMessages;

            // Discount statistics
            var totalCoupons = await _discountStatisticsService.GetTotalDiscountCouponCountAsync();
            var activeCoupons = await _discountStatisticsService.GetActiveDiscountCouponCountAsync();
            var inactiveCoupons = await _discountStatisticsService.GetInactiveDiscountCouponCountAsync();
            var expiredCoupons = await _discountStatisticsService.GetExpiredDiscountCouponCountAsync();
            var validCoupons = await _discountStatisticsService.GetNonExpiredDiscountCouponCountAsync();

            ViewBag.TotalCoupons = totalCoupons;
            ViewBag.ActiveCoupons = activeCoupons;
            ViewBag.InactiveCoupons = inactiveCoupons;
            ViewBag.ExpiredCoupons = expiredCoupons;
            ViewBag.ValidCoupons = validCoupons;

            // Order statistics
            var orders = await _orderService.GetOrderingListAsync();
            var totalOrders = orders.Count;
            var todayOrders = orders.Where(o => o.OrderDate.Date == DateTime.UtcNow.Date).ToList();
            var todayRevenue = todayOrders.Sum(o => o.TotalPrice);
            var totalRevenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalPrice);
            var pendingOrders = orders.Count(o => o.Status == "New" || o.Status == "Processing");
            var deliveredOrders = orders.Count(o => o.Status == "Delivered");
            
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TodayOrders = todayOrders.Count;
            ViewBag.TodayRevenue = todayRevenue;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.DeliveredOrders = deliveredOrders;
            ViewBag.RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).ToList();

            // Chart data - Last 7 days sales
            var salesData = new List<decimal>();
            var salesLabels = new List<string>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                var dayRevenue = orders.Where(o => o.OrderDate.Date == date).Sum(o => o.TotalPrice);
                salesData.Add(dayRevenue);
                salesLabels.Add(date.ToString("ddd"));
            }
            ViewBag.SalesData = salesData;
            ViewBag.SalesLabels = salesLabels;

            // Order status distribution
            var statusCounts = new Dictionary<string, int>
            {
                { "New", orders.Count(o => o.Status == "New") },
                { "Processing", orders.Count(o => o.Status == "Processing") },
                { "Shipped", orders.Count(o => o.Status == "Shipped") },
                { "Delivered", orders.Count(o => o.Status == "Delivered") },
                { "Cancelled", orders.Count(o => o.Status == "Cancelled") }
            };
            ViewBag.StatusCounts = statusCounts;

            // User mapping for orders
            try
            {
                var users = await _userIdentityService.GetAllUsersAsync();
                var map = users?.GroupBy(u => u.Id).ToDictionary(g => g.Key, g => g.First().FullName ?? string.Empty) ?? new Dictionary<string, string>();
                ViewBag.UserMap = map;
            }
            catch { ViewBag.UserMap = new Dictionary<string, string>(); }

            var currentTemp = await FetchCurrentTempAsync("ankara");
            ViewBag.Weather = currentTemp;

            var forecast = await FetchForecastAsync("ankara", currentTemp);
            ViewBag.ForecastTemps = forecast.temps;
            ViewBag.ForecastIcons = forecast.icons;
            ViewBag.Days = forecast.days;

            return View();
        }

        private async Task<double?> FetchCurrentTempAsync(string location, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://yahoo-weather5.p.rapidapi.com/weather?location={Uri.EscapeDataString(location)}&format=json&u=c"));
                request.Headers.Add("x-rapidapi-key", "");
                //cb64a79606msh09533d50616a846p1c5e79jsne442b60c4edf
                request.Headers.Add("x-rapidapi-host", "yahoo-weather5.p.rapidapi.com");
                using var response = await client.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(body);
                var rootEl = doc.RootElement;
                if (rootEl.TryGetProperty("current_observation", out var cur) && cur.TryGetProperty("condition", out var cond) && cond.TryGetProperty("temperature", out var tempEl) && tempEl.ValueKind == JsonValueKind.Number)
                {
                    return Math.Round(tempEl.GetDouble(), 1);
                }
            }
            catch { }
            return null;
        }

        private async Task<(List<string> days, List<string> temps, List<string> icons)> FetchForecastAsync(string location, double? currentTempC, CancellationToken cancellationToken = default)
        {
            var temps = new List<string>();
            var daysFromForecast = new List<string>();
            var icons = new List<string>();
            var culture = new System.Globalization.CultureInfo("en-US");

            try
            {
                var client = _httpClientFactory.CreateClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, new Uri($"https://yahoo-weather5.p.rapidapi.com/weather?location={Uri.EscapeDataString(location)}&format=json&u=c"));
                request.Headers.Add("x-rapidapi-key", "");
                //cb64a79606msh09533d50616a846p1c5e79jsne442b60c4edf
                request.Headers.Add("x-rapidapi-host", "yahoo-weather5.p.rapidapi.com");
                using var response = await client.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                using var rootDoc = JsonDocument.Parse(body);
                var root = rootDoc.RootElement;

                var perDayTemps = new Dictionary<DateTime, List<double>>();
                var perDayIcon = new Dictionary<DateTime, string>();

                void addSample(DateTime date, double tempC, string? weatherMain)
                {
                    var key = date.Date;
                    if (!perDayTemps.TryGetValue(key, out var arr)) { arr = new List<double>(); perDayTemps[key] = arr; }
                    arr.Add(tempC);
                    if (weatherMain != null && !perDayIcon.ContainsKey(key))
                    {
                        string iconClass = weatherMain switch
                        {
                            "Clear" => "wi wi-day-sunny",
                            "Clouds" => "wi wi-day-cloudy",
                            "Rain" => "wi wi-rain",
                            "Drizzle" => "wi wi-sprinkle",
                            "Thunderstorm" => "wi wi-thunderstorm",
                            "Snow" => "wi wi-snow",
                            "Mist" => "wi wi-fog",
                            "Fog" => "wi wi-fog",
                            _ => "wi wi-day-cloudy"
                        };
                        perDayIcon[key] = iconClass;
                    }
                }

                if (root.TryGetProperty("forecasts", out var forecasts) && forecasts.ValueKind == JsonValueKind.Array && forecasts.GetArrayLength() > 0)
                {
                    int take = Math.Min(7, forecasts.GetArrayLength());
                    for (int i = 0; i < take; i++)
                    {
                        var f = forecasts[i];
                        int? high = null; string? wText = null; DateTime date = DateTime.Now.Date.AddDays(i);
                        if (f.TryGetProperty("high", out var h) && h.ValueKind == JsonValueKind.Number) high = h.GetInt32();
                        if (f.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String) wText = t.GetString();
                        if (f.TryGetProperty("date", out var d) && d.ValueKind == JsonValueKind.Number)
                        {
                            var epoch = d.GetInt64(); if (epoch > 1000000000 && epoch < 10000000000) date = DateTimeOffset.FromUnixTimeSeconds(epoch).LocalDateTime.Date;
                        }
                        if (high.HasValue) addSample(date, high.Value, wText);
                    }
                }

                // Open-format fallbacks (list/forecast/daily/data)
                if (root.TryGetProperty("list", out var listEl) && listEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in listEl.EnumerateArray())
                    {
                        DateTime dtLocal = DateTime.Now;
                        if (item.TryGetProperty("dt", out var dtEl)) dtLocal = DateTimeOffset.FromUnixTimeSeconds(dtEl.GetInt64()).LocalDateTime;
                        else if (item.TryGetProperty("dt_txt", out var dtTxtEl) && dtTxtEl.ValueKind == JsonValueKind.String && DateTime.TryParse(dtTxtEl.GetString(), out var parsedDt)) dtLocal = parsedDt;
                        if (item.TryGetProperty("main", out var mainNode) && mainNode.TryGetProperty("temp", out var tNode) && tNode.ValueKind == JsonValueKind.Number)
                        { var val = tNode.GetDouble(); addSample(dtLocal, val > 80 ? val - 273.15 : val, null); }
                    }
                }
                if (root.TryGetProperty("forecast", out var fcNode) && fcNode.TryGetProperty("forecastday", out var fdays) && fdays.ValueKind == JsonValueKind.Array)
                {
                    foreach (var d in fdays.EnumerateArray())
                    {
                        DateTime dtLocal = DateTime.Now; if (d.TryGetProperty("date", out var dateStr) && dateStr.ValueKind == JsonValueKind.String && DateTime.TryParse(dateStr.GetString(), out var parsedDt)) dtLocal = parsedDt;
                        if (d.TryGetProperty("day", out var dayNode) && dayNode.TryGetProperty("avgtemp_c", out var avgTemp) && avgTemp.ValueKind == JsonValueKind.Number) addSample(dtLocal, avgTemp.GetDouble(), null);
                    }
                }
                if (root.TryGetProperty("daily", out var dailyNode) && dailyNode.ValueKind == JsonValueKind.Array)
                {
                    int take = Math.Min(6, dailyNode.GetArrayLength());
                    for (int i = 0; i < take; i++)
                    {
                        var d = dailyNode[i]; if (d.TryGetProperty("temp", out var tnode) && tnode.TryGetProperty("day", out var dayT) && dayT.ValueKind == JsonValueKind.Number) addSample(DateTime.Now.Date.AddDays(i), dayT.GetDouble() - 273.15, null);
                    }
                }

                var todayDate = DateTime.Now.Date;
                var orderedDates = perDayTemps.Keys.Where(d => d > todayDate).OrderBy(d => d).Take(6).ToList();
                foreach (var d in orderedDates)
                {
                    var avg = perDayTemps[d].Average(); temps.Add(Math.Round(avg).ToString()); daysFromForecast.Add(d.ToString("ddd", culture)); icons.Add(perDayIcon.TryGetValue(d, out var ic) ? ic : "wi wi-day-cloudy");
                }
                while (temps.Count < 6)
                {
                    var baseDate = orderedDates.Count > 0 ? orderedDates.Last() : todayDate; var nextDate = baseDate.AddDays(1);
                    temps.Add("-"); daysFromForecast.Add(nextDate.ToString("ddd", culture)); icons.Add("wi wi-day-cloudy"); orderedDates.Add(nextDate);
                }

                var lastVal = temps.LastOrDefault(t => t != "-") ?? (currentTempC.HasValue ? Math.Round(currentTempC.Value).ToString() : null);
                if (lastVal != null) { for (int i = 0; i < temps.Count; i++) if (temps[i] == "-") temps[i] = lastVal; }
                if (icons.Count == 0) icons = new List<string> { "wi wi-day-sunny", "wi wi-day-cloudy", "wi wi-day-cloudy", "wi wi-day-cloudy", "wi wi-day-cloudy", "wi wi-day-cloudy" };
            }
            catch { }

            return (daysFromForecast, temps, icons);
        }
    }
}
