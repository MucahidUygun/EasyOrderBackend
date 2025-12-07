using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
using Application.Services.CorporateCustomers;
using AutoMapper;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Mailing;
using Core.Security.Hashing;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;
using MimeKit;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Registers.RegisterCustomers;

public class RegisterCorporateCustomerCommand : IRequest<RegisteredResponse>
{
    public RegisterCorporateCustomerRequest? RegisterCorporateCustomerRequests { get; set; }
    public string IpAdress { get; set; }

    public RegisterCorporateCustomerCommand()
    {
        RegisterCorporateCustomerRequests = null;
        IpAdress = string.Empty;
    }

    public RegisterCorporateCustomerCommand(RegisterCorporateCustomerRequest registerCorporateCustomerRequests, string ipAdress)
    {
        RegisterCorporateCustomerRequests = registerCorporateCustomerRequests;
        IpAdress = ipAdress;
    }

    public class RegisterCorporateCustomerCommandHandler : IRequestHandler<RegisterCorporateCustomerCommand, RegisteredResponse>
    {
        public readonly IUserOperationClaimRepository _userOperationClaimRepository;
        public readonly IMapper _mapper;
        public readonly IAuthService _authService;
        public readonly ICorporateCustomerService _corporateCustomerService;
        private readonly IMailService _mailService;

        public RegisterCorporateCustomerCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            IMapper mapper,
            IAuthService authService,
            ICorporateCustomerService corporateCustomerService,
            IMailService mailService)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
            _authService = authService;
            _corporateCustomerService = corporateCustomerService;
            _mailService = mailService;
        }

        public async Task<RegisteredResponse> Handle(RegisterCorporateCustomerCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterCorporateCustomerRequests is null)
                throw new BusinessException("Reqeust not be null");
            CorporateCustomer corporateCustomer = _mapper.Map<CorporateCustomer>(request.RegisterCorporateCustomerRequests);

            HashingHelper.CreatePasswordHash
                (
                password:request.RegisterCorporateCustomerRequests.Password!,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
                );
            corporateCustomer.PasswordHash = passwordHash;
            corporateCustomer.PasswordSalt = passwordSalt;
            await _corporateCustomerService.AddAsync( corporateCustomer,IsActive:false,cancellationToken:cancellationToken );

            //UserOperationClaim[] userCliams =
            //{
            //    new() { UserId = corporateCustomer.Id, OperationClaimId = 100,IsActive=true },
            //    new() { UserId = corporateCustomer.Id, OperationClaimId = 101,IsActive=true },
            //};

            foreach (int roleId in AuthOperationClaims.OperationClaimCorporateCustomerRoleIds.ToArray())
            {
                UserOperationClaim claim = new UserOperationClaim 
                {
                    IsActive= true,
                    OperationClaimId = roleId,
                    UserId = corporateCustomer.Id
                };
                await _userOperationClaimRepository.AddAsync(claim);
            }

            //AccessToken accessToken = await _authService.CreateAccessToken(corporateCustomer);
            //BaseRefreshToken refreshToken = await _authService.CreateRefreshToken(corporateCustomer, request.IpAdress);
            //BaseRefreshToken addedRefreshToken = await _authService.AddRefreshToken(refreshToken);
            //accessToken.Expiration = DateTime.Now.AddMinutes(10);

            EmailAuthenticator emailAuthenticator = await _authService.CreateEmailVerifyAsync( corporateCustomer );
            string urlActivationKey = CustomEncoders.UrlEncode(emailAuthenticator.ActivationKey!);

            var toEmailList = new List<MailboxAddress> { new(name: corporateCustomer.CompanyName, corporateCustomer.Email) };

            string VeriFyLink = $"http://localhost:7064/api/Auth/verifyAccount?Id={corporateCustomer.Id}&ActivationKey={urlActivationKey}";

            string htmlFilePath = Path.Combine("wwwroot", "emails", "VerifyEmail.html");

            string htmlContent = File.ReadAllText(htmlFilePath);

            htmlContent = htmlContent.Replace("{{VerifyLink}}", VeriFyLink);

            _mailService.SendMail(
                new Mail
                {
                    ToList = toEmailList,
                    Subject = "Hesap aktivasyon - EasyOrder",
                    TextBody =
                        $"Link Üzerinde Hesabızı Onaylayın :  {VeriFyLink}",
                    HtmlBody = htmlContent
                }
            );

            return new RegisteredResponse() { Message=AuthMessages.SendEmailForEmailActivate };
        }
    }
}
