using Sisk.Core.Http;
using Sisk.Core.Routing;

var server = HttpServer.Emit(0, out HttpServerConfiguration config, out ListeningHost host, out Router router);
config.AccessLogsStream = null;

router += new Route(RouteMethod.Get, "/", request => new HttpResponse().WithContent("Hello, world!"));

server.Start();

System.Console.WriteLine($"The Sisk HTTP server is listening at {server.ListeningPrefixes[0]}");
System.Threading.Thread.Sleep(-1);