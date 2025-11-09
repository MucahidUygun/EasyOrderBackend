using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class BaseClaim : BaseEntity<int>
{
    public string Name { get; set; }

    public BaseClaim()
    {
        
    }
    public BaseClaim(string name)
    {
        Name = name;
    }
}
