using LightJson;
using Sisk.Core.Http;
using Sisk.Core.Routing;

namespace quickstart.crud_api;

public abstract class JsonApiController : RouterModule
{
}

public abstract class JsonApiController<TModel> : JsonApiController where TModel : notnull
{
    public abstract Task<TModel?> Get(Guid id);
    public abstract Task<IEnumerable<TModel>> List();
    public abstract Task<Guid?> Create(TModel model);

    [RouteGet()]
    public async Task<HttpResponse> GetAll(HttpRequest request)
    {
        var items = await this.List();
        return this.Ok(null, items);
    }

    [RouteGet("<id>")]
    public async Task<HttpResponse> GetModel(HttpRequest request)
    {
        Guid id = request.Query["id"].GetGuid();

        var model = await this.Get(id);

        if (model is null)
        {
            return this.NotFound();
        }

        return this.Ok(null, model);
    }

    [RoutePost()]
    public async Task<HttpResponse> CreateModel(HttpRequest request)
    {
        string requestBody = request.Body;

        TModel modelBody = JsonValue.Deserialize(requestBody).Get<TModel>();
        var state = await this.Create(modelBody);

        if (state is Guid id)
        {
            return this.Ok("Model added successfully.", new { id });
        }
        else
        {
            return this.BadRequest("Couldn't create the model.");
        }
    }

    protected HttpResponse NotFound() => Program.CreateJsonResponse(false, "Entity not found", null).WithStatus(404);

    protected HttpResponse BadRequest(string message) => Program.CreateJsonResponse(false, message, null).WithStatus(403);

    protected HttpResponse Ok(string? message, object? data) => Program.CreateJsonResponse(true, message, data);
}
