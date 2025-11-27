using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class BaseEmailAuthenticators<TUserId>:BaseEntity<TUserId>
{
    public TUserId UserId { get; set; }
    public string? ActivationKey { get; set; }

    public BaseEmailAuthenticators()
    {
        UserId = default(TUserId);
    }
    public BaseEmailAuthenticators(TUserId userId,bool isActive)
    {
        UserId =userId;
        IsActive = isActive;
    }
    public BaseEmailAuthenticators(TUserId id,TUserId userId,bool isActive):base(id)
    {
        UserId = userId;
        IsActive = isActive;
    }
}
