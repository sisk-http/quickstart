using quickstart.crud_api;
using quickstart.crud_api.Model;
using Sisk.Core.Http;
using System.Text;

LightJson.JsonOptions.Default.PropertyNameComparer = StringComparer.CurrentCultureIgnoreCase;

var appBuilder = HttpServer.CreateBuilder(host =>
{
    host.UseListeningPort(9500);
    host.UseRouter(router =>
    {
        router.AutoScanModules<JsonApiController>();
        router.CallbackErrorHandler = (ex, r) =>
        {
            return CreateJsonResponse(false, ex.Message, null).WithStatus(403);
        };
    });
});

appBuilder.Build().Start();

partial class Program
{
    public static Repository<Blog> Blogs = new Repository<Blog>();
    public static Repository<Post> Posts = new Repository<Post>();

    public static HttpResponse CreateJsonResponse(bool success, string? message, object? data)
    {
        var apiResponse = new
        {
            success,
            message,
            data
        };

        string json = LightJson.JsonValue.Serialize(apiResponse).ToString();

        return new HttpResponse().WithContent(new StringContent(json, Encoding.UTF8, "application/json"));
    }
}