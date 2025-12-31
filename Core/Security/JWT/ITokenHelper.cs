using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security.JWT;

public interface ITokenHelper
{
    public AccessToken CreateToken(BaseUser user,IList<BaseClaim> operationClaims);

    public BaseRefreshToken CreateRefreshToken(
        BaseUser user,
        string ipAdress, 
        string deviceId, 
        string? deviceName, 
        string userAgent, 
        string devicePlatform);
}
