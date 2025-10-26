using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class FuelType: BaseEntity<int>
{
    public string Name { get; set; }
    public virtual ICollection<Model> Models { get; set; }

    public FuelType()
    {
        Models = new HashSet<Model>();
    }
    public FuelType(string name)
    {
        Name = name;
    }
}
