using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Position : BaseEntity<Guid>
{
    public string PositionName { get; set; }
}
