using System.ComponentModel.DataAnnotations;

namespace quickstart.crud_api.Model;

public class Blog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(64)]
    public required string Name { get; set; }
}
