using Sisk.Core.Http;
using Sisk.Core.Routing;

namespace quickstart.handlebars_example;

class Program
{
    static void Main(string[] args)
    {
        HttpServer server = HttpServer.Emit(0, out HttpServerConfiguration config, out ListeningHost host, out Router router);

        config.DefaultCultureInfo = new System.Globalization.CultureInfo("en-US");
        router.MatchRoutesIgnoreCase = true;

        router += new Route(RouteMethod.Get, "/", request =>
        {
            var page = Templater.Render("view.index", new { datetime = DateTime.Now.ToString("D") });
            return new HttpResponse().WithContent(new HtmlContent(page));
        });
        router += new Route(RouteMethod.Get, "/assets/(?<path>.*)", request =>
        {
            var path = request.Query["path"]!;
            Stream? assetStream = Templater.GetAssetStream("assets." + path);

            if (assetStream == null)
                return new HttpResponse(404);

            string contentType = Templater.GetContentMimeType(path);
            var rs = request.GetResponseStream();
            rs.SetHeader("Content-Type", contentType);
            rs.SetHeader("Content-Length", assetStream.Length.ToString());

            assetStream.CopyTo(rs.ResponseStream);

            return rs.Close();
        })
        { UseRegex = true };

        Templater.InitializeTemplater();
        server.Start();

        Console.WriteLine($"Sisk is listening at {server.ListeningPrefixes[0]}");

        Thread.Sleep(-1);
    }
}
