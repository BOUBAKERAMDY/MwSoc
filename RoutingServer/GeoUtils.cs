using System;

namespace LetsGoBiking.RoutingServer
{
    public static class GeoUtils
    {
        private const double EARTH_RADIUS_KM = 6371.0;
        private const double WALK_SPEED_MS = 1.4; 
        private const double BIKE_SPEED_MS = 4.4;

        public static double WalkSpeedMs => WALK_SPEED_MS;
        public static double BikeSpeedMs => BIKE_SPEED_MS;

        public static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return EARTH_RADIUS_KM * c * 1000; 
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
        public static double WalkingSecondsFromMeters(double meters)
        {
            return meters / WALK_SPEED_MS;
        }

        public static double BikingSecondsFromMeters(double meters)
        {
            return meters / BIKE_SPEED_MS;
        }

        public static double SecondsFromMeters(double meters, bool isBiking)
        {
            return isBiking ? BikingSecondsFromMeters(meters) : WalkingSecondsFromMeters(meters);
        }
    }
}
