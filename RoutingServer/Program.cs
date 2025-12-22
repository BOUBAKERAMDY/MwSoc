using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace LetsGoBiking.RoutingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseRestUrl = "http://localhost:8090/MyService";
            string baseSoapUrl = "http://localhost:8091/MyServiceSoap";

            WebServiceHost restHost = null;
            ServiceHost soapHost = null;

            try
            {
                // ============================================================
                // 1) REST HOST (WebHttpBinding)
                // ============================================================
                restHost = new WebServiceHost(typeof(ItineraryService), new Uri(baseRestUrl));

                var restEndpoint = restHost.AddServiceEndpoint(
                    typeof(IItineraryService),
                    new WebHttpBinding(),
                    ""
                );

                var restBehavior = restEndpoint.EndpointBehaviors
                    .OfType<WebHttpBehavior>()
                    .FirstOrDefault();

                if (restBehavior == null)
                {
                    restBehavior = new WebHttpBehavior
                    {
                        HelpEnabled = true,
                        AutomaticFormatSelectionEnabled = false,
                        DefaultOutgoingResponseFormat = WebMessageFormat.Json
                    };
                    restEndpoint.EndpointBehaviors.Add(restBehavior);
                }

                // Enable WSDL help page
                var restMetadata = restHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (restMetadata == null)
                {
                    restMetadata = new ServiceMetadataBehavior { HttpGetEnabled = true };
                    restHost.Description.Behaviors.Add(restMetadata);
                }

                // ============================================================
                // 2) SOAP HOST (BasicHttpBinding)
                // ============================================================
                soapHost = new ServiceHost(typeof(ItineraryService), new Uri(baseSoapUrl));

                soapHost.AddServiceEndpoint(
                    typeof(IItineraryService),
                    new BasicHttpBinding(),
                    ""
                );

                var soapMetadata = soapHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (soapMetadata == null)
                {
                    soapMetadata = new ServiceMetadataBehavior { HttpGetEnabled = true };
                    soapHost.Description.Behaviors.Add(soapMetadata);
                }

                // Enable ?wsdl
                soapHost.AddServiceEndpoint(
                    ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexHttpBinding(),
                    "mex"
                );

                // ============================================================
                // START BOTH HOSTS
                // ============================================================
                restHost.Open();
                soapHost.Open();

                Console.WriteLine("==============================================");
                Console.WriteLine("     ROUTING SERVER - SOAP + REST ENABLED");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                Console.WriteLine("REST endpoint:");
                Console.WriteLine($"  GET {baseRestUrl}/Itinerary?origin=...&destination=...");
                Console.WriteLine($"  GET {baseRestUrl}/ItineraryByCoords?originLat=...&originLon=...&destLat=...&destLon=...");
                Console.WriteLine();
                Console.WriteLine("SOAP endpoint:");
                Console.WriteLine($"  WSDL → {baseSoapUrl}?wsdl");
                Console.WriteLine();
                Console.WriteLine("Press Enter to stop...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("ERROR starting Routing Server:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("==============================================");
                Console.ReadLine();
            }
            finally
            {
                restHost?.Close();
                soapHost?.Close();
            }
        }
    }
}