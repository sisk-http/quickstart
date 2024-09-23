using quickstart.crud_api.Interfaces;
using System.ComponentModel.DataAnnotations;

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
}
