using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public abstract class BaseUserOperationClaim<TUserId,TClaimId>:BaseEntity<Guid>
{
    public TUserId UserId { get; set; }
    public TClaimId OperationClaimId { get; set; }

    public virtual BaseUser User { get; set; }
    public virtual BaseClaim OperationClaim { get; set; }
    public BaseUserOperationClaim()
    {
        
    }

    public BaseUserOperationClaim(Guid id, TUserId userId, TClaimId claimId) : base(id)
    {
        UserId = userId;
        OperationClaimId = claimId;
    }
}
