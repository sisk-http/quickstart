using quickstart.crud_api;
using quickstart.crud_api.Controller;
using quickstart.crud_api.Model;
using Sisk.Core.Http;
using System.Text;

LightJson.JsonOptions.Default.Converters.Add(new Blog.JsonBlogConverter());
LightJson.JsonOptions.Default.Converters.Add(new Post.JsonPostConverter());
LightJson.JsonOptions.Default.PropertyNameCaseInsensitive = true;

var appHost = HttpServer.CreateBuilder(host =>
{
    host.UseListeningPort(9500);
    host.UseRouter(router =>
    {
        router.SetObject(new BlogController());
        router.CallbackErrorHandler = (ex, r) =>
        {
            return CreateJsonResponse(false, ex.Message, null).WithStatus(403);
        };
    });
});

appHost.Start();

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