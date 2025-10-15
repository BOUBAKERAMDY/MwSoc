using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
public static class Geocoder
{
    private static readonly HttpClient http = new HttpClient();

    public static async Task<(double lat, double lon)> GetCoordinatesAsync(string address)
    {
        try
        {
            var encoded = Uri.EscapeDataString(address);
            var url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=json&limit=1";

            http.DefaultRequestHeaders.UserAgent.ParseAdd("Let'sGoBikingApp/1.0");

            var json = await http.GetStringAsync(url);
            var results = JsonSerializer.Deserialize<NominatimResult[]>(json);

            if (results != null && results.Length > 0)
            {
                double lat = double.Parse(results[0].lat, System.Globalization.CultureInfo.InvariantCulture);
                double lon = double.Parse(results[0].lon, System.Globalization.CultureInfo.InvariantCulture);
                return (lat, lon);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Geocoding failed: {ex.Message}");
        }

        // fallback si no encuentra nada
        return (0, 0);
    }

    private class NominatimResult
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }
}