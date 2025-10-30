using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Order.Application.Features.Mediator.Commands.OrderingCommands;
using MultiShop.Order.Application.Features.Mediator.Queries.OrderingQueries;

namespace MultiShop.Order.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly MultiShop.Order.Application.Interfaces.IOrderDetailRepository _orderDetailRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public OrderingsController(IMediator mediator, MultiShop.Order.Application.Interfaces.IOrderDetailRepository orderDetailRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _mediator = mediator;
            _orderDetailRepository = orderDetailRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> OrderingList()
        {
            var values = await _mediator.Send(new GetOrderingQuery());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderingById(int id)
        {
            var values = await _mediator.Send(new GetOrderingByIdQuery(id));
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrdering(CreateOrderingCommand command)
        {
            var createdId = await _mediator.Send(command);
            return Ok(createdId);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveOrdering(int id)
        {
            await _mediator.Send(new RemoveOrderingCommand(id));
            return Ok("Order successfully deleted");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrdering(UpdateOrderingCommand command)
        {
            await _mediator.Send(command);
            return Ok("Order successfully updated");
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            try
            {
                var root0 = Directory.GetCurrentDirectory();
                var logDir0 = Path.Combine(root0, "wwwroot", "logs");
                Directory.CreateDirectory(logDir0);
                var logFile0 = Path.Combine(logDir0, "order-decrement.log");
                System.IO.File.AppendAllLines(logFile0, new[] { $"{DateTime.UtcNow:O}\tEntering UpdateStatus id={id} status={status}" });
            }
            catch { }
            await _mediator.Send(new UpdateOrderingStatusCommand(id, status));
            if (string.Equals(status, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                var details = await _orderDetailRepository.GetAllAsync();
                var items = details.Where(d => d.OrderingId == id).ToList();
                var baseUrl = _configuration["Services:CatalogBaseUrl"];
                try
                {
                    var root1 = Directory.GetCurrentDirectory();
                    var logDir1 = Path.Combine(root1, "wwwroot", "logs");
                    Directory.CreateDirectory(logDir1);
                    var logFile1 = Path.Combine(logDir1, "order-decrement.log");
                    System.IO.File.AppendAllLines(logFile1, new[] { $"{DateTime.UtcNow:O}\tPreCall items={items.Count} baseUrl={baseUrl}" });
                }
                catch { }
                if (!string.IsNullOrWhiteSpace(baseUrl) && items.Count > 0)
                {
                    // Accept dev certs for local gateway in development
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    using var client = new HttpClient(handler);
                    // Acquire client credentials token for Ocelot/Catalog scope
                    try
                    {
                        var tokenClient = _httpClientFactory.CreateClient();
                        var tokenUrl = _configuration["IdentityServerUrl"].TrimEnd('/') + "/connect/token";
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string,string>("grant_type","client_credentials"),
                            new KeyValuePair<string,string>("client_id","MultiShopVisitorId"),
                            new KeyValuePair<string,string>("client_secret","multishopvisitorsecret"),
                            new KeyValuePair<string,string>("scope","OcelotFullPermission CatalogFullPermission")
                        });
                        var tokenResp = await tokenClient.PostAsync(tokenUrl, content);
                        try
                        {
                            var root01 = Directory.GetCurrentDirectory();
                            var logDir01 = Path.Combine(root01, "wwwroot", "logs");
                            Directory.CreateDirectory(logDir01);
                            var logFile01 = Path.Combine(logDir01, "order-decrement.log");
                            var respBody01 = await tokenResp.Content.ReadAsStringAsync();
                            System.IO.File.AppendAllLines(logFile01, new[] { $"{DateTime.UtcNow:O}\tTokenResp: {(int)tokenResp.StatusCode} body={respBody01}" });
                        }
                        catch { }
                        if (tokenResp.IsSuccessStatusCode)
                        {
                            var json = await tokenResp.Content.ReadAsStringAsync();
                            var accessToken = System.Text.Json.JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
                            if (!string.IsNullOrWhiteSpace(accessToken))
                            {
                                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                                try
                                {
                                    var root02 = Directory.GetCurrentDirectory();
                                    var logDir02 = Path.Combine(root02, "wwwroot", "logs");
                                    Directory.CreateDirectory(logDir02);
                                    var logFile02 = Path.Combine(logDir02, "order-decrement.log");
                                    System.IO.File.AppendAllLines(logFile02, new[] { $"{DateTime.UtcNow:O}\tToken obtained: {accessToken.Substring(0, Math.Min(20, accessToken.Length))}..." });
                                }
                                catch { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            var root03 = Directory.GetCurrentDirectory();
                            var logDir03 = Path.Combine(root03, "wwwroot", "logs");
                            Directory.CreateDirectory(logDir03);
                            var logFile03 = Path.Combine(logDir03, "order-decrement.log");
                            System.IO.File.AppendAllLines(logFile03, new[] { $"{DateTime.UtcNow:O}\tToken exception: {ex.Message}" });
                        }
                        catch { }
                    }
                    var url = baseUrl.TrimEnd('/') + "/Products/DecrementStock";
                    foreach (var it in items)
                    {
                        try
                        {
                            var payload = new { ProductId = it.ProductId, Quantity = it.ProductAmount };
                            using var resp = await System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync(client, url, payload);
                            try
                            {
                                var root = Directory.GetCurrentDirectory();
                                var logDir = Path.Combine(root, "wwwroot", "logs");
                                Directory.CreateDirectory(logDir);
                                var logFile = Path.Combine(logDir, "order-decrement.log");
                                var line = $"{DateTime.UtcNow:O}\tOrder {id}\tProduct {it.ProductId}\tQty {it.ProductAmount}\tStatus {(int)resp.StatusCode}";
                                System.IO.File.AppendAllLines(logFile, new[] { line });
                            }
                            catch { }
                        }
                        catch { }
                    }
                }
            }
            return Ok("Status updated");
        }

        [AllowAnonymous]
        [HttpGet("decrement-test/{id}")]
        public async Task<IActionResult> DecrementTest(int id)
        {
            var details = await _orderDetailRepository.GetAllAsync();
            var items = details.Where(d => d.OrderingId == id).ToList();
            var baseUrl = _configuration["Services:CatalogBaseUrl"];
            try
            {
                var root0 = Directory.GetCurrentDirectory();
                var logDir0 = Path.Combine(root0, "wwwroot", "logs");
                Directory.CreateDirectory(logDir0);
                var logFile0 = Path.Combine(logDir0, "order-decrement.log");
                System.IO.File.AppendAllLines(logFile0, new[] { $"{DateTime.UtcNow:O}\tDecrementTest entry id={id} items={items.Count} baseUrl={baseUrl}" });
                if (items.Count > 0)
                {
                    foreach (var it in items)
                    {
                        System.IO.File.AppendAllLines(logFile0, new[] { $"{DateTime.UtcNow:O}\t\tProductId={it.ProductId} Quantity={it.ProductAmount}" });
                    }
                }
            }
            catch { }
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var client = new HttpClient(handler);
            try
            {
                var tokenClient = _httpClientFactory.CreateClient();
                var tokenUrl = _configuration["IdentityServerUrl"].TrimEnd('/') + "/connect/token";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type","client_credentials"),
                    new KeyValuePair<string,string>("client_id","MultiShopVisitorId"),
                    new KeyValuePair<string,string>("client_secret","multishopvisitorsecret"),
                    new KeyValuePair<string,string>("scope","OcelotFullPermission CatalogFullPermission")
                });
                var tokenResp = await tokenClient.PostAsync(tokenUrl, content);
                if (tokenResp.IsSuccessStatusCode)
                {
                    var json = await tokenResp.Content.ReadAsStringAsync();
                    var accessToken = System.Text.Json.JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    }
                }
                else
                {
                    try
                    {
                        var root1 = Directory.GetCurrentDirectory();
                        var logDir1 = Path.Combine(root1, "wwwroot", "logs");
                        Directory.CreateDirectory(logDir1);
                        var logFile1 = Path.Combine(logDir1, "order-decrement.log");
                        System.IO.File.AppendAllLines(logFile1, new[] { $"{DateTime.UtcNow:O}\tToken request failed: {(int)tokenResp.StatusCode}" });
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var root2 = Directory.GetCurrentDirectory();
                    var logDir2 = Path.Combine(root2, "wwwroot", "logs");
                    Directory.CreateDirectory(logDir2);
                    var logFile2 = Path.Combine(logDir2, "order-decrement.log");
                    System.IO.File.AppendAllLines(logFile2, new[] { $"{DateTime.UtcNow:O}\tToken exception: {ex.Message}" });
                }
                catch { }
            }
            var url = baseUrl.TrimEnd('/') + "/Products/DecrementStock";
            foreach (var it in items)
            {
                var payload = new { ProductId = it.ProductId, Quantity = it.ProductAmount };
                using var resp = await System.Net.Http.Json.HttpClientJsonExtensions.PostAsJsonAsync(client, url, payload);
                try
                {
                    var root3 = Directory.GetCurrentDirectory();
                    var logDir3 = Path.Combine(root3, "wwwroot", "logs");
                    Directory.CreateDirectory(logDir3);
                    var logFile3 = Path.Combine(logDir3, "order-decrement.log");
                    var respBody = await resp.Content.ReadAsStringAsync();
                    System.IO.File.AppendAllLines(logFile3, new[] { $"{DateTime.UtcNow:O}\tResponse: {(int)resp.StatusCode} body={respBody}" });
                }
                catch { }
            }
            return Ok(new { id, items = items.Count });
        }

        [HttpGet("GetOrderingByUserId/{id}")]
        public async Task<IActionResult> GetOrderingByUserId(string id)
        {
            var values = await _mediator.Send(new GetOrderingByUserIdQuery(id));
            return Ok(values);
        }
    }
}