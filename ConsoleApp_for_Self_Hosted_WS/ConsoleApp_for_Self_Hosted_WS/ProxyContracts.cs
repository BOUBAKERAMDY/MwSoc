using System.Runtime.Serialization;
using System.ServiceModel;

namespace ProxyContracts
{
    [DataContract]
    public class ItineraryDto
    {
        [DataMember] public string StartStation { get; set; }
        [DataMember] public string EndStation { get; set; }
        [DataMember] public int WalkToStartSeconds { get; set; }
        [DataMember] public int BikeSeconds { get; set; }
        [DataMember] public int WalkToEndSeconds { get; set; }
        [DataMember] public double TotalKm { get; set; }
    }

    [ServiceContract]
    public interface IProxyService
    {
        [OperationContract]
        ItineraryDto GetItinerary(string origin, string destination);
    }
}