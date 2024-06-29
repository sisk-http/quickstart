using Sisk.Core.Http;
using Sisk.Core.Routing;

namespace quickstart.minimal_hello;

class Program
{
    static void Main(string[] args)
    {
        using var host = HttpServer.CreateBuilder()
            .UseListeningPort(5523)
            .Build();

        host.Router.SetRoute(RouteMethod.Get, "/", request =>
        {
            return new HttpResponse()
                .WithContent(new HtmlContent("<h1>Hello, world!</h1>"));
        });

        host.Start();
    }
}
