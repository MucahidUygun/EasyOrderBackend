using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class RefreshToken : BaseRefreshToken
{
    public virtual User User { get; set; }    
}
