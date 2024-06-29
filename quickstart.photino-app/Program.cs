using Photino.NET;
using Sisk.Core.Http;
using Sisk.Core.Routing;
using System.Reflection;
using System.Runtime.InteropServices;

// running Photino on windows requires an STA main
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
    Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
}

var currentAssembly = Assembly.GetExecutingAssembly();
var currentResources = currentAssembly.GetManifestResourceNames();

using var host = HttpServer.CreateBuilder()
    .UseConfiguration(config =>
    {
        config.AccessLogsStream = LogStream.ConsoleOutput;
    })
    .Build();

host.Router.SetRoute(RouteMethod.Any, Route.AnyPath, ServeStaticFile);

host.Start(preventHault: false);

CreatePhotinoWindow(host.HttpServer.ListeningPrefixes[0])
    .WaitForClose();

HttpResponse ServeStaticFile(HttpRequest request)
{
    string requestedFile = request.Path == "/" ? ".index.html" :
        request.Path.Replace('/', '.');

    Stream? fileStream = GetResourceStream(requestedFile);
    if (fileStream is null)
    {
        return new HttpResponse(404)
            .WithContent(new HtmlContent("<h1>The requested file couldn't be found.</h1>"));
    }

    return new HttpResponse(200)
        .WithHeader("Content-Type", GetMimeType(requestedFile))
        .WithContent(new StreamContent(fileStream));
}

Stream? GetResourceStream(string resourceName)
{
    foreach (string resource in currentResources)
    {
        if (resource.EndsWith("wwwroot" + resourceName, StringComparison.CurrentCultureIgnoreCase))
        {
            return currentAssembly.GetManifestResourceStream(resource);
        }
    }

    return null;
}

PhotinoWindow CreatePhotinoWindow(string url)
{
    var ph = new PhotinoWindow();
    ph.Centered = true;
    ph.Size = new System.Drawing.Size(700, 700);
    ph.UseOsDefaultSize = false;
    ph.WebSecurityEnabled = false;
    ph.StartUrl = url;

    return ph;
}

string GetMimeType(string resourceName)
{
    string ext = Path.GetExtension(resourceName).ToLower();
    return ext switch
    {
        ".html" or ".htm" => "text/html",
        ".css" => "text/css",
        ".js" => "text/javascript",
        _ => "application/octet-stream"
    };
}