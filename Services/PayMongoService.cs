using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using IT15_Final_Proj.Models;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

namespace IT15_Final_Proj.Services
{
    public class PayMongoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;

        public PayMongoService(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _secretKey = config["PayMongo:SecretKey"];
            var byteArray = Encoding.ASCII.GetBytes(_secretKey + ":");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<string> CreateCheckoutLinkAsync(string fullName, string email, long amountInCentavos, int quantity, string description, int requestId)
        {
            var payload = new
            {
                data = new
                {
                    attributes = new
                    {
                        billing = new { name = fullName, email = email },
                        send_email_receipt = true,
                        show_description = true,
                        show_line_items = true,
                        description = $"{description} x{quantity}",
                        line_items = new[]
                        {
                            new {
                                currency = "PHP",
                                amount = amountInCentavos,
                                name = description,
                                quantity = quantity
                            }
                        },
                        payment_method_types = new[] { "gcash", "card" },
                        success_url = $"https://localhost:7220/Vendor/Transactions?requestId={requestId}",
                        cancel_url = "https://localhost:7220/Vendor/Products"
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.paymongo.com/v1/checkout_sessions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create PayMongo checkout: " + responseString);

            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement.GetProperty("data").GetProperty("attributes").GetProperty("checkout_url").GetString();
        }
        public async Task<string> CreateCustomerCheckoutLinkAsync(
    string fullName,
    string email,
    long amountInCentavos,
    string description,
    List<PayMongoLineItem> lineItems)
        {
            var payload = new
            {
                data = new
                {
                    attributes = new
                    {
                        billing = new { name = fullName, email = email },
                        send_email_receipt = true,
                        show_description = true,
                        show_line_items = true,
                        description = description,
                        line_items = lineItems.Select(item => new
                        {
                            currency = "PHP",
                            amount = item.Amount,
                            name = item.Name,
                            quantity = item.Quantity
                        }).ToArray(),
                        payment_method_types = new[] { "gcash", "card" },
                        success_url = "https://localhost:7220/Customer/OrderSuccess",
                        cancel_url = "https://localhost:7220/Customer/CustomerCart"
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.paymongo.com/v1/checkout_sessions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create PayMongo checkout: " + responseString);

            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement.GetProperty("data").GetProperty("attributes").GetProperty("checkout_url").GetString();
        }

    }
}
