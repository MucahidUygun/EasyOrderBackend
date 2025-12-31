using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class RefreshTokenFactory : IRefreshTokenFactory
{
    public IConfiguration Configuration { get; }
    private readonly TokenOptions? _tokenOptions;

    public RefreshTokenFactory(IConfiguration configuration)
    {
        Configuration = configuration;
        _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
    }

    public BaseRefreshToken Create(BaseUser user, string ipAdress, string deviceId, string? deviceName, string userAgent, string devicePlatform)
    {
        return new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresDate = DateTime.UtcNow.AddDays(_tokenOptions!.RefreshTokenTTL),
            DeviceId = deviceId,
            DeviceName = deviceName,
            UserAgent = userAgent,
            DevicePlatform = devicePlatform,
            CreatedByIp = ipAdress,
        };
    }
}
