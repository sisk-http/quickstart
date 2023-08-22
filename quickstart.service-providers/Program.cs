using Sisk.ServiceProvider;
using System.Collections.Specialized;

namespace quickstart.service_providers;

class Program
{
    public static NameValueCollection AppParameters { get; set; } = null!;

    static void Main(string[] args)
    {
        Factory appFactory = new Factory();
        ServiceProvider provider = new ServiceProvider(appFactory);

        // configure and start the program
        provider.ConfigureInit(builder =>
        {
            builder.UseLocale(new System.Globalization.CultureInfo("en-US"));
            builder.UseConfiguration(config =>
            {
                config.ThrowExceptions = false; // for production
            });
        });
    }
}
