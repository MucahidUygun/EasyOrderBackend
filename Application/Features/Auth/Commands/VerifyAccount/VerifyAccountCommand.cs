using Application.Features.Auth.Dtos.Requests;
using Application.Services.AuthService;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.VerifyAccount;

public class VerifyAccountCommand : IRequest
{
    public VerifyEmailCommandRequest? VerifyEmailCommandRequest { get; set; }
    public class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand>
    {
        private readonly IAuthService _authService;

        public VerifyAccountCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
        {
            if (request.VerifyEmailCommandRequest is null && request.VerifyEmailCommandRequest?.ActivationKey is null)
                throw new BusinessException("Request is null");
            EmailAuthenticator? authenticator = await _authService.VeriyfEmailAsync(Id:request.VerifyEmailCommandRequest.Id,activationKey:request.VerifyEmailCommandRequest.ActivationKey);

            if (authenticator is null)
                throw new AuthorizationException("ActivationKey is invalid");
            if (authenticator.IsActive == true)
                throw new AuthorizationException("Email already verified");
            if (authenticator.VerifyEmailTokenExpiry < DateTime.Now)
                throw new AuthorizationException("Link expired");

            await _authService.VerifiedEmailAsync(authenticator:authenticator);
        }
    }
}
