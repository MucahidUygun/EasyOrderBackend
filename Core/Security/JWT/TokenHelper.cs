using Core.Entities;
using Core.Security.Encryption;
using Core.Security.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security.JWT;

public class TokenHelper : ITokenHelper
{
    public IConfiguration Configuration { get;}
    private readonly TokenOptions _tokenOptions;
    private DateTime _accessTokenExpiration;

    public TokenHelper(IConfiguration configuration)
    {
        Configuration = configuration;
        _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
    }

    public AccessToken CreateToken(BaseUser user, IList<BaseClaim> operationClaims)
    {
        _accessTokenExpiration = DateTime.Now.AddSeconds(_tokenOptions.AccessTokenExpiration);
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
        JwtSecurityToken jwt = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwt);


        return new AccessToken()
        {
            Token = token,
            Expiration = _accessTokenExpiration,
        };
    }

    public BaseRefreshToken CreateRefreshToken(BaseUser user, string ipAdress)
    {
        BaseRefreshToken refreshToken = new()
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresDate = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTTL),
            CreatedByIp = ipAdress,
        };

        return refreshToken;
    }

    public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, BaseUser user,
                                                   SigningCredentials signingCredentials,
                                                   IList<BaseClaim> operationClaims)
    {
        JwtSecurityToken jwt = new(
            tokenOptions.Issuer,
            tokenOptions.Audience,
            expires: _accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: SetClaims(user, operationClaims),
            signingCredentials: signingCredentials
        );
        return jwt;
    }

    private IEnumerable<Claim> SetClaims(BaseUser user, IList<BaseClaim> operationClaims)
    {
        List<Claim> claims = new();
        claims.AddNameIdentifier(user.Id.ToString());
        claims.AddEmail(user.Email);
        //claims.AddName($"{user.FirstName} {user.LastName}");
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());
        return claims;
    }


}
