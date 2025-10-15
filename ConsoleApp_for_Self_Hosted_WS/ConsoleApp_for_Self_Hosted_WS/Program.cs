using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
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
                ep.Behaviors.Add(new CorsEnabledBehavior());

                host.Open();
                Console.WriteLine("REST service running at: " + baseAddress);
                Console.WriteLine("Try: " + baseAddress + "/Itinerary?origin=A&destination=B");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}

#region CORS inline
class CorsMessageInspector : IDispatchMessageInspector
{
    public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext) => null;
    public void BeforeSendReply(ref Message reply, object correlationState)
    {
        var ctx = WebOperationContext.Current;
        if (ctx != null)
        {
            var h = ctx.OutgoingResponse.Headers;
            h["Access-Control-Allow-Origin"] = "*";
            h["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
            h["Access-Control-Allow-Headers"] = "Content-Type";
        }
    }
}

class CorsEnabledBehavior : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint ep, BindingParameterCollection bp) { }
    public void ApplyClientBehavior(ServiceEndpoint ep, ClientRuntime cr) { }
    public void ApplyDispatchBehavior(ServiceEndpoint ep, EndpointDispatcher ed)
        => ed.DispatchRuntime.MessageInspectors.Add(new CorsMessageInspector());
    public void Validate(ServiceEndpoint ep) { }
}
#endregion