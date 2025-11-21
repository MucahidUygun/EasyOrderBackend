using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
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

        public LogoutCommandHandler(IAuthService authManager)
        {
            _authManager = authManager;
        }

        public async Task<ExitedResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _authManager.LogOut();
        }
    }
}
