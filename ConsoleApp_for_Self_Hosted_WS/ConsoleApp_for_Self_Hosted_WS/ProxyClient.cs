using System;
using System.ServiceModel;
using ProxyContracts;

public static class ProxyClient
{
    private static readonly BasicHttpBinding Binding = new BasicHttpBinding
    {
        MaxReceivedMessageSize = 8 * 1024 * 1024,
        OpenTimeout = TimeSpan.FromSeconds(5),
        SendTimeout = TimeSpan.FromSeconds(20),
        ReceiveTimeout = TimeSpan.FromSeconds(20)
    };

    // ⚠️ TEMPORAL: cambia esta URL cuando tu compi te diga la suya
    private static readonly EndpointAddress Address =
        new EndpointAddress("http://localhost:9000/ProxyService");

    private static readonly ChannelFactory<IProxyService> Factory =
        new ChannelFactory<IProxyService>(Binding, Address);

    public static IProxyService Create() => Factory.CreateChannel();
}