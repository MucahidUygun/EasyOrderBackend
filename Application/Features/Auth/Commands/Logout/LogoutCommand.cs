using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Responses;
using Application.Features.Auth.Rules;
using Application.Services.AuthService;
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

        public LogoutCommandHandler(IAuthService authManager, AuthBusinessRules authBusinessRules)
        {
            _authManager = authManager;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<ExitedResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            string? refreshToken = _authManager.GetRefreshTokenFromCookie();
            _authBusinessRules.RefreshTokenIsHave(refreshToken);
            string? ipAdress = _authManager.GetByIpAdressFromHeaders();
            _authBusinessRules.IpAdressIsHave(ipAdress);
            BaseRefreshToken? baseRefresh = await _authManager.GetRefreshTokenAsync(p => p.Token == refreshToken && p.CreatedByIp == ipAdress,cancellationToken:cancellationToken);
            _authBusinessRules.RefreshTokenIsHave(baseRefresh);
            await _authManager.RevokeDescendantRefreshTokens(baseRefresh!, ipAdress!, AuthMessages.LogOut);
            _authManager.DeleteRefreshTokenFromCookie();
            return new() { Message = AuthMessages.LogOutSuccess, Status = true };
        }
    }
}
