using AutoMapper;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper _tokenHelper;
    //private readonly IUserClaimRepository;
    private readonly TokenOptions _tokenOptions;
    private readonly IMapper _mapper;

    public AuthManager(IRefreshTokenRepository refreshTokenRepository, ITokenHelper tokenHelper, TokenOptions tokenOptions, IMapper mapper, IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions = 
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration");
    }

    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<AccessToken> CreateAccessToken(User user)
    {
        //AccessToken accessToken = _tokenHelper.CreateToken(user);

        throw new NotImplementedException();
    }

    public Task<RefreshToken> CreateRefreshToken(User user, string ipAdress)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOldRefreshToken(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetRefreshTokenByToken(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        throw new NotImplementedException();
    }


}
