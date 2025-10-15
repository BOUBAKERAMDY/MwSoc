using System;
using System.Collections.Generic;
using System.Linq;
using ProxyContracts;

public static class ItineraryPlanner
{
    public static ItineraryDto Compute(
        double originLat, double originLon,
        double destLat, double destLon,
        IEnumerable<StationInfo> stations,
        double maxWalkToStartMeters = 1200,
        double maxWalkToEndMeters = 1200,
        string note = null)
    {
        var list = stations?.ToList() ?? new List<StationInfo>();
        if (list.Count == 0)
            return WalkOnly(originLat, originLon, destLat, destLon, note ?? "No stations available.");

        var start = list.Where(s => s.Bikes > 0)
                        .Select(s => new { S = s, D = GeoUtils.HaversineMeters(originLat, originLon, s.Lat, s.Lng) })
                        .OrderBy(x => x.D).FirstOrDefault();

        var end = list.Where(s => s.Stands > 0)
                      .Select(s => new { S = s, D = GeoUtils.HaversineMeters(destLat, destLon, s.Lat, s.Lng) })
                      .OrderBy(x => x.D).FirstOrDefault();

        if (start == null || end == null)
            return WalkOnly(originLat, originLon, destLat, destLon, note ?? "No start/end station available.");

        if (start.S == end.S)
        {
            end = list.Where(s => s != start.S && s.Stands > 0)
                      .Select(s => new { S = s, D = GeoUtils.HaversineMeters(destLat, destLon, s.Lat, s.Lng) })
                      .OrderBy(x => x.D).FirstOrDefault()
                  ?? end;
        }

        if (start.D > maxWalkToStartMeters || end.D > maxWalkToEndMeters)
            return WalkOnly(originLat, originLon, destLat, destLon, note ?? "Walking to stations too long.");

        var walk1 = start.D;
        var bike = GeoUtils.HaversineMeters(start.S.Lat, start.S.Lng, end.S.Lat, end.S.Lng);
        var walk2 = end.D;

        var tWalk1 = GeoUtils.SecondsFromMeters(walk1, GeoUtils.WalkMps);
        var tBike = GeoUtils.SecondsFromMeters(bike, GeoUtils.BikeMps);
        var tWalk2 = GeoUtils.SecondsFromMeters(walk2, GeoUtils.WalkMps);

        var totalMeters = walk1 + bike + walk2;

        return new ItineraryDto
        {
            StartStation = start.S.Name,
            EndStation = end.S.Name,

            WalkToStartMeters = Math.Round(walk1, 1),
            BikeMeters = Math.Round(bike, 1),
            WalkToEndMeters = Math.Round(walk2, 1),

            WalkToStartSeconds = tWalk1,
            BikeSeconds = tBike,
            WalkToEndSeconds = tWalk2,

            TotalKm = Math.Round(totalMeters / 1000.0, 3),
            TotalSeconds = tWalk1 + tBike + tWalk2,
            Note = note ?? "Computed locally (Haversine, fixed speeds)."
        };
    }

    private static ItineraryDto WalkOnly(double olat, double olon, double dlat, double dlon, string note)
    {
        var m = GeoUtils.HaversineMeters(olat, olon, dlat, dlon);
        return new ItineraryDto
        {
            StartStation = null,
            EndStation = null,
            WalkToStartMeters = 0,
            BikeMeters = 0,
            WalkToEndMeters = Math.Round(m, 1),

            WalkToStartSeconds = 0,
            BikeSeconds = 0,
            WalkToEndSeconds = GeoUtils.SecondsFromMeters(m, GeoUtils.WalkMps),

            TotalKm = Math.Round(m / 1000.0, 3),
            TotalSeconds = GeoUtils.SecondsFromMeters(m, GeoUtils.WalkMps),
            Note = note ?? "Walk-only fallback."
        };
    }
}