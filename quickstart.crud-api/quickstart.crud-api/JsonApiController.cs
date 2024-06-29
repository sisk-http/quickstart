using LightJson;
using Sisk.Core.Http;
using Sisk.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quickstart.crud_api;

public abstract class JsonApiController<TModel> : RouterModule
{
    public abstract Task<TModel?> Get(Guid id);
    public abstract Task<IEnumerable<TModel>> List();
    public abstract Task<Guid?> Create(TModel model);

    [RouteGet()]
    public async Task<HttpResponse> GetAll(HttpRequest request)
    {
        var items = await List();
        return Ok(null, items);
    }

    [RouteGet("<id>")]
    public async Task<HttpResponse> GetModel(HttpRequest request)
    {
        Guid id = request.Query["id"].GetGuid();

        var model = await Get(id);

        if (model is null)
        {
            return NotFound();
        }

        return Ok(null, model);
    }

    [RoutePost()]
    public async Task<HttpResponse> CreateModel(HttpRequest request)
    {
        string requestBody = request.Body;

        TModel modelBody = JsonValue.Deserialize(requestBody).Get<TModel>();
        var state = await Create(modelBody);

        if (state is Guid id)
        {
            return Ok("Model added successfully.", new { id });
        }
        else
        {
            return BadRequest("Couldn't create the model.");
        }
    }

    protected HttpResponse NotFound() => Program.CreateJsonResponse(false, "Entity not found", null).WithStatus(404);

    protected HttpResponse BadRequest(string message) => Program.CreateJsonResponse(false, message, null).WithStatus(403);

    protected HttpResponse Ok(string? message, object? data) => Program.CreateJsonResponse(true, message, data);
}
