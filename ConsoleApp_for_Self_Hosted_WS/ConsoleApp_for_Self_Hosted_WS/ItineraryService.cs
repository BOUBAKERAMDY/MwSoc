using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using ProxyContracts;

namespace ConsoleApp_for_Self_Hosted_WS
{
    public class ItineraryService : IItineraryService
    {
        public ItineraryDto GetItinerary(string origin, string destination)
        {
            try
            {
                origin = NormalizeComma(Uri.UnescapeDataString(origin ?? "").Trim());
                destination = NormalizeComma(Uri.UnescapeDataString(destination ?? "").Trim());

                var (oLat, oLon) = GetCoordsWithRetry(origin);
                var (dLat, dLon) = GetCoordsWithRetry(destination);

                if ((oLat == 0 && oLon == 0) || (dLat == 0 && dLon == 0))
                {
                    return ItineraryPlanner.Compute(
                        oLat, oLon, dLat, dLon,
                        Array.Empty<StationInfo>(),
                        maxWalkToStartMeters: 1200,
                        maxWalkToEndMeters: 1200,
                        note: "Geocoding failed or zero coords.");
                }

                Console.WriteLine($"[Itinerary] origin({oLat},{oLon})  dest({dLat},{dLon})");

                var straight = GeoUtils.HaversineMeters(oLat, oLon, dLat, dLon);
                if (straight > 150_000 && !(origin.Contains(",") && destination.Contains(",")))
                {
                    return new ItineraryDto
                    {
                        Note = "Geocoding looks wrong (too far). Check addresses.",
                        TotalKm = Math.Round(straight / 1000.0, 3),
                        TotalSeconds = GeoUtils.SecondsFromMeters(straight, GeoUtils.WalkMps)
                    };
                }

                StationInfo[] stations = Array.Empty<StationInfo>();
                var proxy = ProxyClient.Create();
                try
                {
                    stations = proxy.GetStations("Nice");
                    ((IClientChannel)proxy).Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Proxy] " + ex.Message);
                    var ch = (IClientChannel)proxy;
                    if (ch.State != CommunicationState.Closed) ch.Abort();

                    stations = new[]
                    {
                        new StationInfo { Name="Jean Médecin", Lat=43.7010, Lng=7.2680, Bikes=6, Stands=2 },
                        new StationInfo { Name="Massena",      Lat=43.6950, Lng=7.2830, Bikes=3, Stands=5 },
                        new StationInfo { Name="Gambetta",     Lat=43.7018, Lng=7.2589, Bikes=0, Stands=7 }
                    };
                }

                return ItineraryPlanner.Compute(
                    oLat, oLon, dLat, dLon,
                    stations,
                    maxWalkToStartMeters: 1200,
                    maxWalkToEndMeters: 1200,
                    note: $"stations={stations?.Length ?? 0}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Itinerary] error: " + ex.Message);
                return new ItineraryDto
                {
                    StartStation = null,
                    EndStation = null,
                    WalkToStartMeters = 0,
                    BikeMeters = 0,
                    WalkToEndMeters = 0,
                    WalkToStartSeconds = 0,
                    BikeSeconds = 0,
                    WalkToEndSeconds = 0,
                    TotalKm = 0,
                    TotalSeconds = 0,
                    Note = "Unhandled error in ItineraryService: " + ex.Message
                };
            }
        }

        // ---------- Helpers privados ----------

        // Normaliza "43.7, 7.2" -> "43.7,7.2"
        private static string NormalizeComma(string s) =>
            string.IsNullOrWhiteSpace(s) ? "" : Regex.Replace(s, @"\s*,\s*", ",");

        // Devuelve (lat,lon). Si es texto y el geocoder falla, reintenta con "Nice, France".
        private static (double lat, double lon) GetCoordsWithRetry(string input)
        {
            if (TryParseLatLon(input, out double lat, out double lon))
                return (lat, lon);

            var a = Geocoder.GetCoordinatesAsync(input).GetAwaiter().GetResult();
            if (a.lat != 0 || a.lon != 0) return a;

            var b = Geocoder.GetCoordinatesAsync(input + ", Nice, France").GetAwaiter().GetResult();
            if (b.lat != 0 || b.lon != 0) return b;

            var c = Geocoder.GetCoordinatesAsync("Nice, France").GetAwaiter().GetResult();
            return (c.lat == 0 && c.lon == 0) ? (0, 0) : c;
        }

        // Acepta "lat,lon" con punto o coma
        private static bool TryParseLatLon(string s, out double lat, out double lon)
        {
            lat = 0; lon = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;

            var parts = s.Split(',');
            if (parts.Length != 2) return false;

            var us = CultureInfo.InvariantCulture;
            var p0 = parts[0].Trim().Replace(',', '.');
            var p1 = parts[1].Trim().Replace(',', '.');

            if (!double.TryParse(p0, NumberStyles.Float, us, out lat)) return false;
            if (!double.TryParse(p1, NumberStyles.Float, us, out lon)) return false;

            return true;
        }
    }
}