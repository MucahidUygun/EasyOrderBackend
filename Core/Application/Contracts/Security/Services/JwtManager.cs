using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Services;

public class JwtManager: IJwtService
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public JwtManager(JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    public List<BaseClaim> GetClaimsByKey(string accessToken, string claimName)
    {
        return _jwtSecurityTokenHandler.ReadJwtToken(accessToken).Claims
            .Where(c => c.Type == claimName)   // 🔹 Burada dikkat
            .Select(c => new BaseClaim
            {
                Name = c.Value  // claim value: örn. "Admin"
            })
            .ToList();
    }

    public string GetEmailFromOldAccesToken(string accessToken)
    {
        return _jwtSecurityTokenHandler.ReadJwtToken(accessToken).Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Email)?.Value!;
    }

    public string GetIdFromOldAccesToken(string accessToken)
    {
        return _jwtSecurityTokenHandler.ReadJwtToken(accessToken).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
    }
}
