using System;
using System.Collections.Generic;
using System.Linq;
using LetsGoBiking.Shared;

namespace LetsGoBiking.RoutingServer
{
    public class ItineraryPlanner
    {
        // Supported JCDecaux cities
        private static readonly string[] SupportedCities = new[]
        {
            "lyon", "paris", "rouen", "toulouse", "nancy", "nantes", "amiens",
            "marseille", "lille", "bruxelles", "valence", "cergy-pontoise",
            "creteil", "luxembourg", "mulhouse", "besancon"
        };

        public static bool IsCitySupported(string city)
        {
            if (string.IsNullOrEmpty(city))
                return false;

            return SupportedCities.Contains(city.ToLower());
        }

        public static ItineraryDto ComputeInterCity(double originLat, double originLon, string originCity,
            double destLat, double destLon, string destCity,
            StationInfo[] originStations, StationInfo[] destStations)
        {
            // Create the itinerary object
            var itinerary = new ItineraryDto
            {
                OriginCity = originCity,
                DestinationCity = destCity,
                IsInterCity = !string.Equals(originCity, destCity, StringComparison.OrdinalIgnoreCase),
                OriginLat = originLat,
                OriginLon = originLon,
                DestLat = destLat,
                DestLon = destLon
            };

            // Check which cities have stations
            bool hasOriginStations = originStations != null && originStations.Length > 0;
            bool hasDestStations = destStations != null && destStations.Length > 0;

            // If NEITHER city has stations → WALK ONLY
            if (!hasOriginStations && !hasDestStations)
            {
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            if (hasOriginStations && !hasDestStations)
            {
                return ComputeOnlyOriginHasBikes(originLat, originLon, destLat, destLon,
                    originCity, destCity, originStations);
            }
            if (!hasOriginStations && hasDestStations)
            {
                return ComputeOnlyDestHasBikes(originLat, originLon, destLat, destLon,
                    originCity, destCity, destStations);
            }

            bool isInterCity = !string.Equals(originCity, destCity, StringComparison.OrdinalIgnoreCase);

            if (isInterCity)
            {
                Console.WriteLine($"[Planner] INTER-CITY route detected: {originCity} → {destCity}");
                return ComputeInterCityRoute(originLat, originLon, destLat, destLon,
                    originCity, destCity, originStations, destStations);
            }
            else
            {
                Console.WriteLine($"[Planner] INTRA-CITY route: {originCity}");
                return ComputeIntraCityRoute(originLat, originLon, destLat, destLon,
                    originCity, originStations);
            }
        }

        /// <summary>
        /// NEW: Only origin city has bike-sharing (e.g., Lyon → Paris)
        /// User walks to Lyon station, bikes to edge of Lyon toward Paris, then walks rest to Paris
        /// </summary>
        private static ItineraryDto ComputeOnlyOriginHasBikes(double originLat, double originLon,
            double destLat, double destLon, string originCity, string destCity,
            StationInfo[] originStations)
        {
            var itinerary = new ItineraryDto
            {
                OriginCity = originCity,
                DestinationCity = destCity,
                IsInterCity = true,
                OriginLat = originLat,
                OriginLon = originLon,
                DestLat = destLat,
                DestLon = destLon
            };

            // Find nearest station to origin
            var startStation = FindNearestAvailableStation(originLat, originLon, originStations, true);
            if (startStation == null)
            {
                Console.WriteLine("[Planner] No bikes available in origin city");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // Find edge station in origin city (closest to destination)
            var edgeStation = FindStationClosestToPoint(destLat, destLon, originStations, false);
            if (edgeStation == null)
            {
                Console.WriteLine("[Planner] No edge station found");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // Calculate distances
            var walkToStart = GeoUtils.HaversineDistance(originLat, originLon,
                startStation.Latitude, startStation.Longitude);
            var bikeDistance = GeoUtils.HaversineDistance(startStation.Latitude, startStation.Longitude,
                edgeStation.Latitude, edgeStation.Longitude);
            var walkToEnd = GeoUtils.HaversineDistance(edgeStation.Latitude, edgeStation.Longitude,
                destLat, destLon);

            // Calculate times
            var time1 = GeoUtils.WalkingSecondsFromMeters(walkToStart);
            var time2 = GeoUtils.BikingSecondsFromMeters(bikeDistance);
            var time3 = GeoUtils.WalkingSecondsFromMeters(walkToEnd);

            itinerary.StartStation = startStation.Name;
            itinerary.EndStation = edgeStation.Name;
            itinerary.StartStationDetails = startStation;
            itinerary.EndStationDetails = edgeStation;

            itinerary.WalkToStartMeters = Math.Round(walkToStart, 2);
            itinerary.BikeMeters = Math.Round(bikeDistance, 2);
            itinerary.WalkToEndMeters = Math.Round(walkToEnd, 2);

            itinerary.TotalKm = Math.Round((walkToStart + bikeDistance + walkToEnd) / 1000, 2);
            itinerary.TotalSeconds = Math.Round(time1 + time2 + time3, 0);
            itinerary.IsWalkingOnly = false;

            itinerary.Note = $"Bike-sharing available only in {originCity}. " +
                           $"Route: Walk to {startStation.Name}, bike to edge of {originCity} ({edgeStation.Name}), " +
                           $"then walk {walkToEnd / 1000:F1} km to {destCity}. " +
                           $"⚠️ Consider public transport for the final {walkToEnd / 1000:F1} km.";

            Console.WriteLine($"[Planner] Only origin has bikes: Walk {walkToStart / 1000:F2}km → " +
                            $"Bike {bikeDistance / 1000:F2}km → Walk {walkToEnd / 1000:F2}km");

            return itinerary;
        }

        /// <summary>
        /// NEW: Only destination city has bike-sharing (e.g., Paris → Lyon)
        /// User walks from Paris to edge of Lyon, picks up bike, bikes to near destination, walks final bit
        /// </summary>
        private static ItineraryDto ComputeOnlyDestHasBikes(double originLat, double originLon,
            double destLat, double destLon, string originCity, string destCity,
            StationInfo[] destStations)
        {
            var itinerary = new ItineraryDto
            {
                OriginCity = originCity,
                DestinationCity = destCity,
                IsInterCity = true,
                OriginLat = originLat,
                OriginLon = originLon,
                DestLat = destLat,
                DestLon = destLon
            };

            // Find edge station in destination city (closest to origin)
            var edgeStation = FindStationClosestToPoint(originLat, originLon, destStations, true);
            if (edgeStation == null)
            {
                Console.WriteLine("[Planner] No edge station found");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // Find nearest station to final destination
            var endStation = FindNearestAvailableStation(destLat, destLon, destStations, false);
            if (endStation == null)
            {
                Console.WriteLine("[Planner] No bike stands available in destination city");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // Calculate distances
            var walkToStart = GeoUtils.HaversineDistance(originLat, originLon,
                edgeStation.Latitude, edgeStation.Longitude);
            var bikeDistance = GeoUtils.HaversineDistance(edgeStation.Latitude, edgeStation.Longitude,
                endStation.Latitude, endStation.Longitude);
            var walkToEnd = GeoUtils.HaversineDistance(endStation.Latitude, endStation.Longitude,
                destLat, destLon);

            // Calculate times
            var time1 = GeoUtils.WalkingSecondsFromMeters(walkToStart);
            var time2 = GeoUtils.BikingSecondsFromMeters(bikeDistance);
            var time3 = GeoUtils.WalkingSecondsFromMeters(walkToEnd);

            itinerary.StartStation = edgeStation.Name;
            itinerary.EndStation = endStation.Name;
            itinerary.StartStationDetails = edgeStation;
            itinerary.EndStationDetails = endStation;

            itinerary.WalkToStartMeters = Math.Round(walkToStart, 2);
            itinerary.BikeMeters = Math.Round(bikeDistance, 2);
            itinerary.WalkToEndMeters = Math.Round(walkToEnd, 2);

            itinerary.TotalKm = Math.Round((walkToStart + bikeDistance + walkToEnd) / 1000, 2);
            itinerary.TotalSeconds = Math.Round(time1 + time2 + time3, 0);
            itinerary.IsWalkingOnly = false;

            itinerary.Note = $"Bike-sharing available only in {destCity}. " +
                           $"Route: Walk {walkToStart / 1000:F1} km from {originCity} to edge of {destCity} ({edgeStation.Name}), " +
                           $"bike to {endStation.Name}, then walk to destination. " +
                           $"⚠️ Consider public transport for the initial {walkToStart / 1000:F1} km.";

            Console.WriteLine($"[Planner] Only dest has bikes: Walk {walkToStart / 1000:F2}km → " +
                            $"Bike {bikeDistance / 1000:F2}km → Walk {walkToEnd / 1000:F2}km");

            return itinerary;
        }

        /// <summary>
        /// Inter-city route: Lyon → Toulouse
        /// 1. Walk to nearest station in Lyon
        /// 2. Bike to edge station in Lyon (closest to Toulouse)
        /// 3. Walk from Lyon edge to Toulouse edge (the gap between cities)
        /// 4. Walk to first station in Toulouse (closest from Lyon direction)
        /// 5. Bike to nearest station to destination in Toulouse
        /// 6. Walk to final destination
        /// </summary>
        private static ItineraryDto ComputeInterCityRoute(double originLat, double originLon,
            double destLat, double destLon,
            string originCity, string destCity,
            StationInfo[] originStations, StationInfo[] destStations)
        {
            var itinerary = new ItineraryDto
            {
                OriginCity = originCity,
                DestinationCity = destCity,
                IsInterCity = true,
                OriginLat = originLat,
                OriginLon = originLon,
                DestLat = destLat,
                DestLon = destLon
            };

            // STEP 1: Find nearest station to origin in origin city
            var originStartStation = FindNearestAvailableStation(originLat, originLon, originStations, true);
            if (originStartStation == null)
            {
                Console.WriteLine("[Planner] No bikes available in origin city");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // STEP 2: Find edge station in origin city (closest to destination city)
            var originEdgeStation = FindStationClosestToPoint(destLat, destLon, originStations, false);
            if (originEdgeStation == null)
            {
                Console.WriteLine("[Planner] No edge station found in origin city");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // STEP 3: Find edge station in destination city (closest to origin city)
            var destEdgeStation = FindStationClosestToPoint(originLat, originLon, destStations, true);
            if (destEdgeStation == null)
            {
                Console.WriteLine("[Planner] No edge station found in destination city");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // STEP 4: Find nearest station to final destination in destination city
            var destEndStation = FindNearestAvailableStation(destLat, destLon, destStations, false);
            if (destEndStation == null)
            {
                Console.WriteLine("[Planner] No bike stands available in destination city");
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // Calculate all segments
            // Segment 1: Walk from origin to first station in origin city
            var walkToOriginStart = GeoUtils.HaversineDistance(
                originLat, originLon,
                originStartStation.Latitude, originStartStation.Longitude);

            // Segment 2: Bike from first station to edge station in origin city
            var bikeInOriginCity = GeoUtils.HaversineDistance(
                originStartStation.Latitude, originStartStation.Longitude,
                originEdgeStation.Latitude, originEdgeStation.Longitude);

            // Segment 3: Walk between edge stations (the inter-city gap)
            var walkBetweenCities = GeoUtils.HaversineDistance(
                originEdgeStation.Latitude, originEdgeStation.Longitude,
                destEdgeStation.Latitude, destEdgeStation.Longitude);

            // Segment 4: Bike from edge station to destination station in destination city
            var bikeInDestCity = GeoUtils.HaversineDistance(
                destEdgeStation.Latitude, destEdgeStation.Longitude,
                destEndStation.Latitude, destEndStation.Longitude);

            // Segment 5: Walk from last station to final destination
            var walkToDestEnd = GeoUtils.HaversineDistance(
                destEndStation.Latitude, destEndStation.Longitude,
                destLat, destLon);

            // Calculate times
            var time1 = GeoUtils.WalkingSecondsFromMeters(walkToOriginStart);
            var time2 = GeoUtils.BikingSecondsFromMeters(bikeInOriginCity);
            var time3 = GeoUtils.WalkingSecondsFromMeters(walkBetweenCities);
            var time4 = GeoUtils.BikingSecondsFromMeters(bikeInDestCity);
            var time5 = GeoUtils.WalkingSecondsFromMeters(walkToDestEnd);

            // Store in DTO - we'll s existing fields creatively
            itinerary.StartStation = $"{originStartStation.Name} → {originEdgeStation.Name} ({originCity})";
            itinerary.EndStation = $"{destEdgeStation.Name} → {destEndStation.Name} ({destCity})";

            itinerary.StartStationDetails = originStartStation;
            itinerary.EndStationDetails = destEndStation;

            // Store distances
            itinerary.WalkToStartMeters = Math.Round(walkToOriginStart, 2);
            itinerary.BikeMeters = Math.Round(bikeInOriginCity + bikeInDestCity, 2); // Total bike distance
            itinerary.WalkToEndMeters = Math.Round(walkBetweenCities + walkToDestEnd, 2); // Inter-city walk + final walk

            var totalDistance = walkToOriginStart + bikeInOriginCity + walkBetweenCities + bikeInDestCity + walkToDestEnd;
            var totalTime = time1 + time2 + time3 + time4 + time5;

            itinerary.TotalKm = Math.Round(totalDistance / 1000, 2);
            itinerary.TotalSeconds = Math.Round(totalTime, 0);
            itinerary.IsWalkingOnly = false;

            // Create detailed note
            itinerary.Note = $"Inter-city route from {originCity} to {destCity}:\n" +
                            $"1. Walk {walkToOriginStart / 1000:F2} km to {originStartStation.Name} ({Math.Round(time1 / 60)} min)\n" +
                            $"2. Bike {bikeInOriginCity / 1000:F2} km to edge station {originEdgeStation.Name} in {originCity} ({Math.Round(time2 / 60)} min)\n" +
                            $"3. Walk {walkBetweenCities / 1000:F2} km between cities (⚠️ {Math.Round(time3 / 60)} min / {Math.Round(time3 / 3600, 1)} hours)\n" +
                            $"4. Bike {bikeInDestCity / 1000:F2} km from {destEdgeStation.Name} to {destEndStation.Name} in {destCity} ({Math.Round(time4 / 60)} min)\n" +
                            $"5. Walk {walkToDestEnd / 1000:F2} km to destination ({Math.Round(time5 / 60)} min)\n\n" +
                            $"⚠️ Note: You cannot bike between cities. The {walkBetweenCities / 1000:F1} km walk between cities is not realistic. Consider train/bus.";

            Console.WriteLine($"[Planner] Inter-city breakdown:");
            Console.WriteLine($"  1. Walk to start: {walkToOriginStart / 1000:F2} km");
            Console.WriteLine($"  2. Bike in {originCity}: {bikeInOriginCity / 1000:F2} km");
            Console.WriteLine($"  3. Walk between cities: {walkBetweenCities / 1000:F2} km ⚠️");
            Console.WriteLine($"  4. Bike in {destCity}: {bikeInDestCity / 1000:F2} km");
            Console.WriteLine($"  5. Walk to end: {walkToDestEnd / 1000:F2} km");

            return itinerary;
        }

        /// <summary>
        /// Intra-city route: Regular bike-sharing within one city
        /// </summary>
        private static ItineraryDto ComputeIntraCityRoute(double originLat, double originLon,
            double destLat, double destLon, string city, StationInfo[] stations)
        {
            var itinerary = new ItineraryDto
            {
                OriginCity = city,
                DestinationCity = city,
                IsInterCity = false,
                OriginLat = originLat,
                OriginLon = originLon,
                DestLat = destLat,
                DestLon = destLon
            };

            // Find nearest station with bikes
            var startStation = FindNearestAvailableStation(originLat, originLon, stations, true);
            if (startStation == null)
            {
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            // Find nearest station with stands to destination
            var endStation = FindNearestAvailableStation(destLat, destLon, stations, false);
            if (endStation == null)
            {
                return ComputeWalkingOnly(originLat, originLon, destLat, destLon);
            }

            var walkToStart = GeoUtils.HaversineDistance(originLat, originLon,
                startStation.Latitude, startStation.Longitude);
            var bikeDistance = GeoUtils.HaversineDistance(startStation.Latitude, startStation.Longitude,
                endStation.Latitude, endStation.Longitude);
            var walkToEnd = GeoUtils.HaversineDistance(endStation.Latitude, endStation.Longitude,
                destLat, destLon);

            var time1 = GeoUtils.WalkingSecondsFromMeters(walkToStart);
            var time2 = GeoUtils.BikingSecondsFromMeters(bikeDistance);
            var time3 = GeoUtils.WalkingSecondsFromMeters(walkToEnd);

            itinerary.StartStation = startStation.Name;
            itinerary.EndStation = endStation.Name;
            itinerary.StartStationDetails = startStation;
            itinerary.EndStationDetails = endStation;

            itinerary.WalkToStartMeters = Math.Round(walkToStart, 2);
            itinerary.BikeMeters = Math.Round(bikeDistance, 2);
            itinerary.WalkToEndMeters = Math.Round(walkToEnd, 2);

            itinerary.TotalKm = Math.Round((walkToStart + bikeDistance + walkToEnd) / 1000, 2);
            itinerary.TotalSeconds = Math.Round(time1 + time2 + time3, 0);
            itinerary.IsWalkingOnly = false;

            itinerary.Note = $"Itinerary within {city} using bike-sharing.";

            Console.WriteLine($"[Planner] Intra-city: Walk {walkToStart:F0}m → Bike {bikeDistance:F0}m → Walk {walkToEnd:F0}m");

            return itinerary;
        }

        /// <summary>
        /// Find station closest to origin (with bikes available)
        /// </summary>
        private static StationInfo FindNearestAvailableStation(double lat, double lon,
            StationInfo[] stations, bool needBike)
        {
            if (stations == null || stations.Length == 0)
                return null;

            var availableStations = stations
                .Where(s => string.Equals(s.Status, "OPEN", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(s.Status, "AVAILABLE", StringComparison.OrdinalIgnoreCase) ||
                           string.IsNullOrEmpty(s.Status))
                .Where(s => needBike ? s.AvailableBikes > 0 : s.AvailableStands > 0)
                .OrderBy(s => GeoUtils.HaversineDistance(lat, lon, s.Latitude, s.Longitude))
                .ToList();

            return availableStations.FirstOrDefault();
        }

        /// <summary>
        /// Find edge station: the station in this city that is closest to the other city
        /// For inter-city routing: find station closest to destination coordinates
        /// </summary>
        private static StationInfo FindStationClosestToPoint(double targetLat, double targetLon,
            StationInfo[] stations, bool needBike)
        {
            if (stations == null || stations.Length == 0)
                return null;

            // Find station closest to the target point (usually in the other city)
            var candidate = stations
                .Where(s => string.Equals(s.Status, "OPEN", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(s.Status, "AVAILABLE", StringComparison.OrdinalIgnoreCase) ||
                           string.IsNullOrEmpty(s.Status))
                .Where(s => needBike ? s.AvailableBikes > 0 : s.AvailableStands > 0)
                .OrderBy(s => GeoUtils.HaversineDistance(targetLat, targetLon, s.Latitude, s.Longitude))
                .FirstOrDefault();

            if (candidate != null)
            {
                var distance = GeoUtils.HaversineDistance(targetLat, targetLon,
                    candidate.Latitude, candidate.Longitude);
                Console.WriteLine($"[Planner] Edge station: {candidate.Name} ({distance / 1000:F2} km from target)");
            }

            return candidate;
        }

        public static ItineraryDto ComputeWalkingOnly(double originLat, double originLon,
            double destLat, double destLon)
        {
            var walkDistance = GeoUtils.HaversineDistance(originLat, originLon, destLat, destLon);
            var walkTime = GeoUtils.WalkingSecondsFromMeters(walkDistance);

            return new ItineraryDto
            {
                IsWalkingOnly = true,
                OriginLat = originLat,
                OriginLon = originLon,
                DestLat = destLat,
                DestLon = destLon,
                StartStation = null,
                EndStation = null,
                StartStationDetails = null,
                EndStationDetails = null,
                WalkToStartMeters = Math.Round(walkDistance, 2),
                BikeMeters = 0,
                WalkToEndMeters = 0,
                TotalKm = Math.Round(walkDistance / 1000, 2),
                TotalSeconds = Math.Round(walkTime, 0),
                Note = "Walking-only itinerary (no bike-sharing available)."
            };
        }
    }
}