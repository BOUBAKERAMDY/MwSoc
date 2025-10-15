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
            // 1) Coords de origen/destino (acepta "lat,lon" o geocoding)
            double oLat, oLon, dLat, dLon;

            if (!TryParseLatLon(origin, out oLat, out oLon))
            {
                var o = Geocoder.GetCoordinatesAsync(origin).GetAwaiter().GetResult();
                oLat = o.lat; oLon = o.lon;
            }

            if (!TryParseLatLon(destination, out dLat, out dLon))
            {
                var d = Geocoder.GetCoordinatesAsync(destination).GetAwaiter().GetResult();
                dLat = d.lat; dLon = d.lon;
            }

            if ((oLat == 0 && oLon == 0) || (dLat == 0 && dLon == 0))
            {
                return ItineraryPlanner.Compute(oLat, oLon, dLat, dLon, Array.Empty<StationInfo>(),
                    maxWalkToStartMeters: 1200, maxWalkToEndMeters: 1200,
                    note: "Geocoding failed or zero coords.");
            }

            Console.WriteLine($"Coords -> origin({oLat},{oLon})  dest({dLat},{dLon})");

            // 2) Llamar al proxy SOAP para obtener estaciones
            StationInfo[] stations = Array.Empty<StationInfo>();
            var proxy = ProxyClient.Create();
            try
            {
                stations = proxy.GetStations("Nice"); // TODO: cambia la ciudad si hace falta
                ((IClientChannel)proxy).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Proxy error: " + ex.Message);
                var ch = (IClientChannel)proxy;
                if (ch.State != CommunicationState.Closed) ch.Abort();
            }

            // 3) Calcular y devolver
            return ItineraryPlanner.Compute(oLat, oLon, dLat, dLon, stations,
                maxWalkToStartMeters: 1200, maxWalkToEndMeters: 1200);
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

    // helper: acepta "lat,lon" (punto o coma)
    private static bool TryParseLatLon(string s, out double lat, out double lon)
    {
        lat = 0; lon = 0;
        if (string.IsNullOrWhiteSpace(s)) return false;

        var parts = s.Split(',');
        if (parts.Length != 2) return false;

        var us = CultureInfo.InvariantCulture;
        var p0 = parts[0].Trim().Replace(',', '.');
        var p1 = parts[1].Trim().Replace(',', '.');

        if (!double.TryParse(p0, System.Globalization.NumberStyles.Float, us, out lat)) return false;
        if (!double.TryParse(p1, System.Globalization.NumberStyles.Float, us, out lon)) return false;

        return true;
    }
}