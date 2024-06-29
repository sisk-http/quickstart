using LightJson;
using LightJson.Converters;
using quickstart.crud_api.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quickstart.crud_api.Model;

public class Post : IModelTimestamps
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public required Guid BlogId { get; set; }

    [StringLength(128)]
    public required string PostName { get; set; }

    [StringLength(16_000)]
    public required string PostContents { get; set; }

    public class JsonPostConverter : JsonConverter
    {
        public override bool CanSerialize(Type type)
        {
            return type == typeof(Post);
        }

        public override object Deserialize(JsonValue value, Type requestedType)
        {
            return new Post()
            {
                BlogId = value["blogId"].Get<Guid>(),
                Id = value["id"].Get<Guid>(),
                CreatedAt = value["createdAt"].Get<DateTime>(),
                ModifiedAt = value["modifiedAt"].Get<DateTime>(),
                PostContents = value["data"]["contents"].GetString(),
                PostName = value["data"]["name"].GetString()
            };
        }

        public override JsonValue Serialize(object value)
        {
            Post post = (Post)value;
            var obj = new JsonObject()
            {
                ["id"] = JsonValue.Serialize(post.Id),
                ["blogId"] = JsonValue.Serialize(post.BlogId),
                ["createdAt"] = JsonValue.Serialize(post.CreatedAt),
                ["modifiedAt"] = JsonValue.Serialize(post.ModifiedAt),
                ["data"] = new JsonObject()
                {
                    ["name"] = new JsonValue(post.PostName),
                    ["contents"] = new JsonValue(post.PostContents)
                }.AsJsonValue()
            };
            return obj.AsJsonValue();
        }
    }
}
