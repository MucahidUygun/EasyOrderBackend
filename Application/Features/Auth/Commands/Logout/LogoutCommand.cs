using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Responses;
using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Logout;

public class LogoutCommand:IRequest<ExitedResponse>
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ExitedResponse>
    {
        private readonly IAuthService _authManager;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IHttpService _httpService;
        public LogoutCommandHandler(IAuthService authManager, AuthBusinessRules authBusinessRules, IHttpService httpService)
        {
            _authManager = authManager;
            _authBusinessRules = authBusinessRules;
            _httpService = httpService;
        }

        public async Task<ExitedResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            string? refreshToken = _httpService.GetRefreshTokenFromCookie();
            _authBusinessRules.RefreshTokenIsHave(refreshToken);
            string? ipAdress = _httpService.GetByIpAdressFromHeaders();
            _authBusinessRules.IpAdressIsHave(ipAdress);
            BaseRefreshToken? baseRefresh = await _authManager.GetRefreshTokenAsync(p => p.Token == refreshToken && p.CreatedByIp == ipAdress,cancellationToken:cancellationToken);
            _authBusinessRules.RefreshTokenIsHave(baseRefresh);
            await _authManager.RevokeDescendantRefreshTokens(baseRefresh!, ipAdress!, AuthMessages.LogOut);
            _httpService.DeleteRefreshTokenFromCookie();
            return new() { Message = AuthMessages.LogOutSuccess, Status = true };
        }
    }
}
