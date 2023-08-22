using Sisk.Core.Http;
using Sisk.Core.Routing;
using System.Collections.Specialized;

namespace quickstart.service_providers;

class Factory : RouterFactory
{
    public override void Bootstrap()
    {
        ;
    }

    public override Router BuildRouter()
    {
        Router myRouter = new Router();

        myRouter += new Route(RouteMethod.Get, "/", request =>
        {
            string? name = Program.AppParameters["name"] ?? "world";
            return new HttpResponse()
                .WithContent($"Hello, {name}!");
        });

        return myRouter;
    }

    public override void Setup(NameValueCollection setupParameters)
    {
        Program.AppParameters = setupParameters;
    }
}
