using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
using Application.Services.IndividualCustomers;
using AutoMapper;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Hashing;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Registers.RegisterCustomers;

public class RegisterIndividualCustomerCommand:IRequest<RegisteredResponse>
{
    public RegisterIndiviualCustomerCommandRequest? CommandRequest { get; set; }
    public string IpAdress { get; set; }

    public RegisterIndividualCustomerCommand()
    {
        CommandRequest = null;
        IpAdress = string.Empty;
    }

    public RegisterIndividualCustomerCommand(RegisterIndiviualCustomerCommandRequest commandRequest, string ipAdress)
    {
        CommandRequest = commandRequest;
        IpAdress = ipAdress;
    }

    public class RegisterIndividualCustomerCommandHandler : IRequestHandler<RegisterIndividualCustomerCommand, RegisteredResponse>
    {
        public readonly IUserOperationClaimRepository _userOperationClaimRepository;
        public readonly IMapper _mapper;
        public readonly IAuthService _authService;
        public readonly IIndividualCustomerService _service;

        public RegisterIndividualCustomerCommandHandler(IUserOperationClaimRepository userOperationClaimRepository, IMapper mapper, IAuthService authService, IIndividualCustomerService service)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
            _authService = authService;
            _service = service;
        }

        public async Task<RegisteredResponse> Handle(RegisterIndividualCustomerCommand request, CancellationToken cancellationToken)
        {
            if (request.CommandRequest is null)
                throw new BusinessException("Command Reqeust cannot be null");
            IndividualCustomer individualCustomer = _mapper.Map<IndividualCustomer>(request.CommandRequest);

            HashingHelper.CreatePasswordHash
                (
                password:request.CommandRequest.Password!,
                passwordHash:out byte[] passwordHash,
                passwordSalt:out byte[] passwordSalt
                );
            individualCustomer.PasswordHash = passwordHash;
            individualCustomer.PasswordSalt = passwordSalt;

            await _service.AddAsync(individualCustomer);
            UserOperationClaim[] userCliams =
            {
                new() { UserId = individualCustomer.Id, OperationClaimId = 100,IsActive=true },
                new() { UserId = individualCustomer.Id, OperationClaimId = 101,IsActive=true },
            };

            foreach (var claim in userCliams)
            {
                await _userOperationClaimRepository.AddAsync(claim);
            }

            AccessToken accessToken = await _authService.CreateAccessToken(individualCustomer);
            BaseRefreshToken refreshToken = await _authService.CreateRefreshToken(individualCustomer,request.IpAdress);
            //BaseRefreshToken addedRefreshToken = await _authService.AddRefreshToken(refreshToken);
            RegisteredResponse registeredResponse = new() { Message = AuthMessages.SendEmailForEmailActivate };

            return registeredResponse;
        }
    }

}
