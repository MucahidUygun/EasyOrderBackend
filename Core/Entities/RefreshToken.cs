using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class RefreshToken : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public string CreatedById { get; set; }
    public DateTime Revoked { get; set; }
    public string RevokedById { get; set; }
    public string ReplacedByToken { get; set; }
    public string ReasonRevoked { get; set; }

    public virtual BaseUser User { get; set; }

    public RefreshToken()
    {

    }

    public RefreshToken(Guid userId, string token, DateTime expires, DateTime created, string createdById, DateTime revoked, string revokedById, string replacedByToken, string reasonRevoked, BaseUser user)
    {
        UserId = userId;
        Token = token;
        Expires = expires;
        Created = created;
        CreatedById = createdById;
        Revoked = revoked;
        RevokedById = revokedById;
        ReplacedByToken = replacedByToken;
        ReasonRevoked = reasonRevoked;
        User = user;
    }
}
