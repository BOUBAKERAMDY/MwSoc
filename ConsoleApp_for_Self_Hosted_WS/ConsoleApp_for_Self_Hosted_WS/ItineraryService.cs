using System;
using System.Globalization;
using System.ServiceModel;
using ProxyContracts;

public class ItineraryService : IItineraryService
{
    public ItineraryDto GetItinerary(string origin, string destination)
    {
        try
        {
            // 1) Conseguir coordenadas de origen/destino
            var (oLat, oLon) = TryParseLatLon(origin, out var okO)
                ? TryParseLatLon(origin, out _)
                : Geocoder.GetCoordinatesAsync(origin).GetAwaiter().GetResult();

            var (dLat, dLon) = TryParseLatLon(destination, out var okD)
                ? TryParseLatLon(destination, out _)
                : Geocoder.GetCoordinatesAsync(destination).GetAwaiter().GetResult();

            if ((oLat == 0 && oLon == 0) || (dLat == 0 && dLon == 0))
            {
                // Fallback caminando si no hubo geocoding
                return ItineraryPlannerFallback(oLat, oLon, dLat, dLon, "Geocoding failed or zero coords.");
            }

            Console.WriteLine($"Coords -> origin({oLat},{oLon})  dest({dLat},{dLon})");

            // 2) Llamar al proxy SOAP para obtener estaciones
            var proxy = ProxyClient.Create();
            StationInfo[] stations = Array.Empty<StationInfo>();
            try
            {
                // Cambia "Nice" por la ciudad/código que use vuestro proxy
                stations = proxy.GetStations("Nice");
                ((IClientChannel)proxy).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Proxy error: " + ex.Message);
                var ch = (IClientChannel)proxy;
                if (ch.State != CommunicationState.Closed) ch.Abort();
            }

            // 3) Calcular itinerario local
            var result = ItineraryPlanner.Compute(oLat, oLon, dLat, dLon, stations);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Itinerary error: " + ex.Message);
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

    // -------- helpers --------

    // acepta "lat,lon" (con punto o coma)
    private static (double lat, double lon) TryParseLatLon(string s, out bool ok)
    {
        ok = false;
        if (string.IsNullOrWhiteSpace(s)) return (0, 0);

        var parts = s.Split(',');
        if (parts.Length != 2) return (0, 0);

        var style = NumberStyles.Float;
        var us = CultureInfo.InvariantCulture;

        if (double.TryParse(parts[0].Trim().Replace(',', '.'), style, us, out var lat) &&
            double.TryParse(parts[1].Trim().Replace(',', '.'), style, us, out var lon))
        {
            ok = true;
            return (lat, lon);
        }
        return (0, 0);
    }

    private static ItineraryDto ItineraryPlannerFallback(double oLat, double oLon, double dLat, double dLon, string note)
    {
        // si tienes ItineraryPlanner, úsalo con 0 estaciones para “walk-only”
        return ItineraryPlanner.Compute(oLat, oLon, dLat, dLon, Array.Empty<StationInfo>(), note);
    }
}