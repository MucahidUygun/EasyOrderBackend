using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Security.Hashing;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ResetPasswordCommand;

public class ChangePasswordCommand : IRequest<ChangePasswordCommandResponse>
{
    public Guid Id;
    public string? ActivationKey;
    public ChangePasswordCommandRequest? ResetPasswordRequest;

    public class ResetPasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordCommandResponse>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<ChangePasswordCommandResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (request is null || request?.ResetPasswordRequest is null)
                throw new BusinessException("Please fill in all fields!");
            if (request.ActivationKey is null)
                throw new BusinessException("Unauthorized action!");
            if (request.ResetPasswordRequest.Password is null || request.ResetPasswordRequest.RePassword is null)
                throw new BusinessException("Please fill in all fields!");
            if (request.ResetPasswordRequest.Password != request.ResetPasswordRequest.RePassword)
                throw new BusinessException("Password not match!");
            User? user = await _authService.GetUserAsync(p=>p.Id == request.Id);
            if (user is null)
                throw new BusinessException("Unauthorized action!");

            EmailAuthenticator? emailAuthenticator = await _authService.GetEmailAuthenticatorAsync(p=>p.UserId == user.Id);
            if (emailAuthenticator is null)
                throw new BusinessException("Unauthorized action!");
            if (emailAuthenticator.VerifyEmailTokenExpiry < DateTime.UtcNow)
                throw new BusinessException("The request period has expired.");
            if (emailAuthenticator.ResetPasswordToken is null || emailAuthenticator.ResetPasswordToken == false
                || emailAuthenticator.IsActive is null || emailAuthenticator.IsActive == false)
                throw new BusinessException("Invalid transaction!");
            HashingHelper.CreatePasswordHash
                (
                password:request.ResetPasswordRequest.Password!,
                passwordHash:out byte[] passwordHash,
                passwordSalt:out byte[] passwordSalt
                );
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _authService.UpdateUserAsync(user,cancellationToken);

            emailAuthenticator.ResetPasswordToken = false;
            emailAuthenticator.ActivationKey = HashingHelper.CreateActivationKey(HashingHelper.GenerateRandomKey());
            await _authService.UpdateEmailAuthenticatorAsync(emailAuthenticator);
            
            return new (){Message= "Password change successful." };
        }
    }
}
