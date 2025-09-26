using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class City : BaseEntity<Guid>
{
    public string CityName { get; set; }
}
