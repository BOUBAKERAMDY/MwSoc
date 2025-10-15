using System;

public static class GeoUtils
{
    const double R = 6371000.0; // radio tierra (m)

    public static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
    {
        double ToRad(double d) => d * Math.PI / 180.0;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    // velocidades en m/s
    public const double WalkMps = 1.389; // 5 km/h
    public const double BikeMps = 4.167; // 15 km/h

    public static int SecondsFromMeters(double meters, double mps)
        => (int)Math.Round(meters / mps);
}