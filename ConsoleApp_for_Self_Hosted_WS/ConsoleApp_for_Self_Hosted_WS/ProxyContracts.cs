using System.Runtime.Serialization;
using System.ServiceModel;

namespace ProxyContracts
{
    // === Estación (lo que te devuelve el proxy) ===
    [DataContract]
    public class StationInfo
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public double Lat { get; set; }
        [DataMember] public double Lng { get; set; }
        [DataMember] public int Bikes { get; set; }   // bicis disponibles
        [DataMember] public int Stands { get; set; }  // anclajes libres
    }

    // === DTO de respuesta de tu servicio ===
    [DataContract]
    public class ItineraryDto
    {
        [DataMember] public string StartStation { get; set; }
        [DataMember] public string EndStation { get; set; }

        [DataMember] public double WalkToStartMeters { get; set; }
        [DataMember] public double BikeMeters { get; set; }
        [DataMember] public double WalkToEndMeters { get; set; }

        [DataMember] public int WalkToStartSeconds { get; set; }
        [DataMember] public int BikeSeconds { get; set; }
        [DataMember] public int WalkToEndSeconds { get; set; }

        [DataMember] public double TotalKm { get; set; }
        [DataMember] public int TotalSeconds { get; set; }
        [DataMember] public string Note { get; set; }
    }

    // === Contrato del proxy SOAP de tu colega (mínimo necesario) ===
    [ServiceContract]
    public interface IProxyService
    {
        [OperationContract]
        StationInfo[] GetStations(string city);

        // (Opcional si su proxy ya calcula itinerario)
        // [OperationContract]
        // ItineraryDto GetItinerary(string origin, string destination);
    }
}