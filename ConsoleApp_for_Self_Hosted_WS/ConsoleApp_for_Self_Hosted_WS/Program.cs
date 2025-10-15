using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace ConsoleApp_for_Self_Hosted_WS
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = new Uri("http://localhost:8090/MyService");

            using (var host = new WebServiceHost(typeof(ItineraryService), baseAddress))
            {
                var ep = host.AddServiceEndpoint(typeof(IItineraryService), new WebHttpBinding(), "");
                ep.Behaviors.Add(new WebHttpBehavior
                {
                    HelpEnabled = true,
                    DefaultOutgoingResponseFormat = WebMessageFormat.Json
                });

                host.Open();
                Console.WriteLine("REST service running at: " + baseAddress);
                Console.WriteLine("Try: " + baseAddress + "/Itinerary?origin=A&destination=B");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}