using System.ServiceModel;
using System.ServiceModel.Web;
using ProxyContracts;

[ServiceContract]
public interface IItineraryService
{
    [OperationContract]
    [WebGet(UriTemplate = "/Itinerary?origin={origin}&destination={destination}",
            ResponseFormat = WebMessageFormat.Json)]
    ItineraryDto GetItinerary(string origin, string destination);
}