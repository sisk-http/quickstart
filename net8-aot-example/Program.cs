namespace net8_aot_example;

using Sisk.Core.Http;
using Sisk.Core.Routing;

class Program
{
    static void Main(string[] args)
    {
        HttpServer server = HttpServer.Emit(5555,
            out var config,
            out var host,
            out var router);

        router.SetRoute(RouteMethod.Get, "/", request =>
        {
            return new HttpResponse()
                .WithContent("Hello world from Sisk Framework!");
        });

        server.Start();
        Console.WriteLine($"Sisk is listening at {server.ListeningPrefixes[0]}");
        Thread.Sleep(-1);
    }
}
