using System;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using LetsGoBiking.Shared;

namespace LetsGoBiking.RoutingServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ItineraryService : IItineraryService
    {
        public ItineraryDto GetItinerary(string origin, string destination)
        {
            try
            {
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext.Current.OutgoingResponse.Headers.Add(
                        "Access-Control-Allow-Origin", "*");
                }

                Console.WriteLine($"[GetItinerary] From '{origin}' to '{destination}'");

                origin = NormalizeAddress(origin);
                destination = NormalizeAddress(destination);
                var originGeoTask = Geocoder.GeocodeAddressAsync(origin);
                var destGeoTask = Geocoder.GeocodeAddressAsync(destination);
                Task.WaitAll(originGeoTask, destGeoTask);

                var (originLat, originLon, originCity) = originGeoTask.Result;
                var (destLat, destLon, destCity) = destGeoTask.Result;

               

                StationInfo[] originStations = null;
                StationInfo[] destStations = null;

                try
                {
                    originStations = ProxyClient.GetStations(originCity);
                    Console.WriteLine($"[GetItinerary] Origin ({originCity}): {originStations?.Length ?? 0} stations");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetItinerary] Failed to get stations for {originCity}: {ex.Message}");
                    originStations = new StationInfo[0];
                }

                try
                {
                    destStations = ProxyClient.GetStations(destCity);
                    Console.WriteLine($"[GetItinerary] Destination ({destCity}): {destStations?.Length ?? 0} stations");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetItinerary] Failed to get stations for {destCity}: {ex.Message}");
                    destStations = new StationInfo[0];
                }

                // ✅ Let the planner decide the routing strategy based on which cities have stations
                // It will handle all scenarios:
                // 1. Both cities have stations → full inter-city routing
                // 2. Only origin has stations → hybrid route (bike in origin, walk to dest)
                // 3. Only dest has stations → hybrid route (walk to dest, bike in dest)
                // 4. Neither has stations → walking-only
                var itinerary = ItineraryPlanner.ComputeInterCity(
                    originLat, originLon, originCity,
                    destLat, destLon, destCity,
                    originStations, destStations);

                return itinerary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetItinerary] Error: {ex.Message}");
                return new ItineraryDto
                {
                    Note = $"Error computing itinerary: {ex.Message}",
                    IsWalkingOnly = true
                };
            }
        }

        public ItineraryDto GetItineraryByCoords(string originLat, string originLon,
                                                 string destLat, string destLon)
        {
            try
            {
                // Enable CORS
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext.Current.OutgoingResponse.Headers.Add(
                        "Access-Control-Allow-Origin", "*");
                }

                double oLat = double.Parse(originLat, CultureInfo.InvariantCulture);
                double oLon = double.Parse(originLon, CultureInfo.InvariantCulture);
                double dLat = double.Parse(destLat, CultureInfo.InvariantCulture);
                double dLon = double.Parse(destLon, CultureInfo.InvariantCulture);
                var originAddress = $"{oLat},{oLon}";
                var destAddress = $"{dLat},{dLon}";

                var originGeoTask = Geocoder.GeocodeAddressAsync(originAddress);
                var destGeoTask = Geocoder.GeocodeAddressAsync(destAddress);
                Task.WaitAll(originGeoTask, destGeoTask);

                var (_, __, originCity) = originGeoTask.Result;
                var (_, ___, destCity) = destGeoTask.Result;

                StationInfo[] originStations = null;
                StationInfo[] destStations = null;

                try
                {
                    originStations = ProxyClient.GetStations(originCity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetItineraryByCoords] Failed to get stations for {originCity}: {ex.Message}");
                    originStations = new StationInfo[0];
                }

                try
                {
                    destStations = ProxyClient.GetStations(destCity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetItineraryByCoords] Failed to get stations for {destCity}: {ex.Message}");
                    destStations = new StationInfo[0];
                }

                var itinerary = ItineraryPlanner.ComputeInterCity(
                    oLat, oLon, originCity,
                    dLat, dLon, destCity,
                    originStations, destStations);

                return itinerary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetItineraryByCoords] Error: {ex.Message}");
                return new ItineraryDto
                {
                    Note = $"Error computing itinerary: {ex.Message}",
                    IsWalkingOnly = true
                };
            }
        }

        private string NormalizeAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return address;

            address = address.Trim();
            if (!address.ToLower().Contains("france"))
            {
                address += ", France";
            }

            return address;
        }
    }
}