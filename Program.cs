using Sisk.Provider;

namespace quickstart.Controller
{
    internal class Program
    {
        internal static ServiceProvider? svcProvider;
        internal static string? DefaultName { get; set; }

        static void Main(string[] args)
        {
            MainRouter routerFactory = new MainRouter();
            svcProvider = new ServiceProvider(routerFactory, "config.json");
            svcProvider.Initialize();
            svcProvider.Wait();
        }
    }
}