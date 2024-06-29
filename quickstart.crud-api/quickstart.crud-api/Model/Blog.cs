using LightJson;
using LightJson.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quickstart.crud_api.Model;

public class Blog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(64)]
    public required string Name { get; set; }

    public class JsonBlogConverter : JsonConverter
    {
        public override bool CanSerialize(Type type)
        {
            return type == typeof(Blog);
        }

        public override object Deserialize(JsonValue value, Type requestedType)
        {
            return new Blog()
            {
                Name = value["name"].GetString(),
                Id = value["id"].MaybeNull()?.Get<Guid>() ?? Guid.NewGuid()
            };
        }

        public override JsonValue Serialize(object value)
        {
            Blog blog = (Blog)value;
            var obj = new JsonObject()
            {
                ["name"] = new JsonValue(blog.Name),
                ["id"] = JsonValue.Serialize(blog.Id)
            };
            return obj.AsJsonValue();
        }
    }
}
