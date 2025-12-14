using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Mailing;
using Core.Security.Hashing;
using Domain.Entities;
using MediatR;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ForgotPasswordResponse>
{
    public ForgotPasswordRequest? Request { get; set; }
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IAuthService _authService;
        private readonly IMailService _mailService;

        public ForgotPasswordCommandHandler(IAuthService authService, IMailService mailService)
        {
            _authService = authService;
            _mailService = mailService;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request is null  || request?.Request is null)
                throw new BusinessException("Email field cannot be empty.");
            User? user = await _authService.GetUserAsync(p=>p.Email == request.Request!.Email);
            if (user is null)
                throw new BusinessException("Password reset e-mail sent.");
            EmailAuthenticator? emailAuthenticator = await _authService.GetEmailAuthenticatorAsync(p=>p.UserId == user.Id,cancellationToken:cancellationToken);
            if (emailAuthenticator is null)
                throw new BusinessException("Please activate your account!");
            emailAuthenticator.ResetPasswordToken = true;
            emailAuthenticator.ResetPasswordTokenExpiry = DateTime.Now.AddMinutes(30);
            emailAuthenticator.ActivationKey = HashingHelper.CreateActivationKey(HashingHelper.GenerateRandomKey(6));

            emailAuthenticator = await _authService.UpdateEmailAuthenticatorAsync(emailAuthenticator);
            if (emailAuthenticator is null)
                throw new BusinessException("Password reset e-mail sent.");

            string urlActivationKey = CustomEncoders.UrlEncode(emailAuthenticator.ActivationKey!);

            var toEmailList = new List<MailboxAddress> { new(name: "Dear Customer", user.Email) };

            string ForgotPasswordLink = $"https://localhost:7064/api/Auth/ResetPassword/{user.Id}/{urlActivationKey}";

            string htmlFilePath = Path.Combine("wwwroot", "emails", "ForgotPassword.html");

            string htmlContent = File.ReadAllText(htmlFilePath);


            htmlContent = htmlContent.Replace("{{ResetLink}}", ForgotPasswordLink);
            htmlContent = htmlContent.Replace("{{ExpiryMinutes}}", (emailAuthenticator.ResetPasswordTokenExpiry)?.ToString("yyyy-MM-dd HH:mm"));

            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "Şifre Yenileme - EasyOrder",
                    TextBody =
                        $"Lütfen şifrenizi yenilemek için :  {ForgotPasswordLink}",
                    HtmlBody = htmlContent,
                }
            );

            return new(){Message ="Password reset e-mail sent" };
        }
    }
}
