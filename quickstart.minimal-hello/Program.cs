using Sisk.Core.Http;
using Sisk.Core.Routing;

namespace quickstart.minimal_hello;

class Program
{
    static void Main(string[] args)
    {
        var server = HttpServer.Emit(0, out HttpServerConfiguration config, out ListeningHost host, out Router router);
        config.AccessLogsStream = null;

        router += new Route(RouteMethod.Get, "/", request => new HttpResponse().WithContent("Hello, world!"));

        server.Start();

        Console.WriteLine($"The Sisk HTTP server is listening at {server.ListeningPrefixes[0]}");

        Thread.Sleep(-1);
    }
}
