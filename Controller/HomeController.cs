using Sisk.Core.Http;
using Sisk.Core.Routing;

namespace quickstart.Controller
{
    internal class HomeController
    {
        [Route(RouteMethod.Get, "/")]
        static HttpResponse Index(HttpRequest request)
        {
            string html = $"""
                <body>
                    <h1>Hello world from Sisk!</h1>
                    <form action="/hello" method="POST">
                        <label for="myName">Enter your name:</label>
                        <input id="myName" name="myName" type="text" />
                        <button type="submit">Submit</button>
                    </form>
                </body>
                """;

            HttpResponse indexResponse = new HttpResponse(200);
            indexResponse.Content = new StringContent(html, Program.svcProvider?.ServerConfiguration!.DefaultEncoding, "text/html");

            return indexResponse;
        }

        [Route(RouteMethod.Post, "/hello")]
        static HttpResponse Hello(HttpRequest request)
        {
            string name = request.GetFormContent()["myName"] ?? Program.DefaultName ?? "(unknown)";
            string html = $"""
                <body>
                    <h1>Hello, {name}!</h1>
                </body>
                """;

            HttpResponse indexResponse = new HttpResponse(200);
            indexResponse.Content = new StringContent(html, Program.svcProvider?.ServerConfiguration!.DefaultEncoding, "text/html");

            return indexResponse;
        }
    }
}
