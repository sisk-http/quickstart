using quickstart.Controller;
using Sisk.Core.Routing;
using System.Collections.Specialized;

namespace quickstart
{
    internal class MainRouter : RouterFactory
    {
        private Router mainRouter = new Router();

        public override Router BuildRouter()
        {
            mainRouter.SetObject(typeof(Controller.HomeController));
            return mainRouter;
        }

        public override void Setup(NameValueCollection setupParameters)
        {
            Program.DefaultName = setupParameters["DefaultName"];
        }
    }
}
