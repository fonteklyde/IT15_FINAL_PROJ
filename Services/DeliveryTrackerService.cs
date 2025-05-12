using System.Text.Json;
using IT15_Final_Proj.Models;
using IT15_Final_Proj.Services;
using Microsoft.EntityFrameworkCore;

public class DeliveryTrackerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DeliveryTrackerService> _logger;
    private readonly IConfiguration _configuration;

    public DeliveryTrackerService(
        IServiceProvider provider,
        IHttpClientFactory httpClientFactory,
        ILogger<DeliveryTrackerService> logger,
        IConfiguration configuration)
    {
        _provider = provider;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var ordersOutForDelivery = await db.Orders
                .Where(o => o.Status == "OUT FOR DELIVERY")
                .ToListAsync();

            foreach (var order in ordersOutForDelivery)
            {
                await SimulateDeliveryAsync(order);
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); // Adjust frequency
        }
    }

    private async Task SimulateDeliveryAsync(Order order)
    {
        var vendorLat = 7.065832635396533;
        var vendorLon = 125.59678107178759;

        var apiKey = _configuration["GraphHopper:ApiKey"];
        var routeUrl = $"https://graphhopper.com/api/1/route?point={vendorLat},{vendorLon}&point={order.DeliveryLatitude},{order.DeliveryLongitude}&vehicle=car&locale=en&key={apiKey}&points_encoded=false";

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(routeUrl);
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var route = JsonDocument.Parse(json);
            var coordinates = route.RootElement
                .GetProperty("paths")[0]
                .GetProperty("points")
                .GetProperty("coordinates");

            var routePoints = coordinates.EnumerateArray()
                .Select(coord => (Lat: coord[1].GetDouble(), Lon: coord[0].GetDouble()))
                .ToList();

            await MoveAlongRouteAsync(order.Id, routePoints);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to simulate delivery.");
        }
    }

    private async Task MoveAlongRouteAsync(int orderId, List<(double Lat, double Lon)> routePoints)
    {
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var order = await db.Orders.FindAsync(orderId);
        if (order == null || order.Status != "OUT FOR DELIVERY")
            return;

        // Attach order to the context to track changes
        db.Attach(order);

        foreach (var point in routePoints)
        {
            // Update lat/lon coordinates
            order.CurrentLat = point.Lat;
            order.CurrentLon = point.Lon;

            // Explicitly mark as modified
            db.Entry(order).State = EntityState.Modified;

            await db.SaveChangesAsync();

            await Task.Delay(3000); // Simulate the delivery moving over time
        }

        // Mark order as delivered after all route points are processed
        order.Status = "DELIVERED";
        order.CurrentLat = order.DeliveryLatitude; // Set final delivery coordinates
        order.CurrentLon = order.DeliveryLongitude;

        db.Entry(order).State = EntityState.Modified;

        await db.SaveChangesAsync();
    }
}
