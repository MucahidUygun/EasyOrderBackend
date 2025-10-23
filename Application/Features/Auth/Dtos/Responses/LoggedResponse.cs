using Core.Security.JWT;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Responses;

public class LoggedResponse
{
    public AccessToken AccessToken { get; set; }
    public RefreshToken RefreshToken { get; set; }

    public LoggedResponse()
    {
        
    }

    public LoggedResponse(AccessToken accessToken, RefreshToken refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
