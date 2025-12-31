using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public abstract class BaseRefreshToken : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime? ExpiresDate { get; set; }
    public string DeviceId { get; set; }
    public string DevicePlatform { get; set; }
    public string? DeviceName { get; set; }
    public string UserAgent { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }

    public virtual BaseUser User { get; set; }

    public BaseRefreshToken()
    {

    }

    public BaseRefreshToken(Guid userId, string token, DateTime expires, string createdByIp, DateTime revoked, string revokedByIp, string replacedByToken, string reasonRevoked, BaseUser user, string deviceId, string devicePlatform, string? deviceName, string userAgent)
    {
        UserId = userId;
        Token = token;
        ExpiresDate = expires;
        CreatedByIp = createdByIp;
        RevokedDate = revoked;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
        ReasonRevoked = reasonRevoked;
        User = user;
        DeviceId = deviceId;
        DevicePlatform = devicePlatform;
        DeviceName = deviceName;
        UserAgent = userAgent;
    }
}
