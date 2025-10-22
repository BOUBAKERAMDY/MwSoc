using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public static class Geocoder
{
    private static readonly HttpClient http = new HttpClient();

    public static async Task<(double lat, double lon)> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return (0, 0);

        try
        {
            //  we add "France"
            string query = address.IndexOf("France", StringComparison.OrdinalIgnoreCase) >= 0
                ? address
                : $"{address}, France";

            string encoded = Uri.EscapeDataString(query);

            string url =
                $"https://nominatim.openstreetmap.org/search?" +
                $"q={encoded}&format=json&limit=1" +
                $"&countrycodes=fr&accept-language=en";

            http.DefaultRequestHeaders.UserAgent.Clear();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("LetsGoBikingApp/1.0 (contact: test@example.com)");

            string json = await http.GetStringAsync(url);
            var results = JsonSerializer.Deserialize<NominatimResult[]>(json);

            if (results != null && results.Length > 0)
            {
                double lat = double.Parse(results[0].lat, CultureInfo.InvariantCulture);
                double lon = double.Parse(results[0].lon, CultureInfo.InvariantCulture);

                Console.WriteLine($"[Geocoder] OK: {address} -> ({lat},{lon}) ✅");
                return (lat, lon);
            }
            else
            {
                Console.WriteLine($"[Geocoder] No results for {address}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Geocoder] Error: {ex.Message}");
        }

        return (0, 0);
    }

    private class NominatimResult
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }
}