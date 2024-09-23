using quickstart.crud_api.Model;
using Sisk.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quickstart.crud_api.Controller;

[RoutePrefix("/blog")]
internal class BlogController : JsonApiController<Blog>
{
    public override async Task<Guid?> Create(Blog model)
    {
        await Task.Run(() => Program.Blogs.Add(model));
        return model.Id;
    }

    public override async Task<Blog?> Get(Guid id)
    {
        return await Task.Run(() => Program.Blogs.FirstOrDefault(b => b.Id == id));
    }

    public override async Task<IEnumerable<Blog>> List()
    {
        return await Task.Run(() => Program.Blogs);
    }
}
