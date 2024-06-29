using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quickstart.crud_api.Interfaces;

public interface IModelTimestamps
{
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
