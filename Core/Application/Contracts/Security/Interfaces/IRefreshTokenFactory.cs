using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IRefreshTokenFactory
{
    BaseRefreshToken Create(BaseUser user, string ipAdress, string deviceId, string? deviceName, string userAgent, string devicePlatform);
}
