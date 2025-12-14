using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
using Application.Services.Customers;
using AutoMapper;
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

namespace Application.Features.Auth.Commands.Registers.RegisterCustomer;

public class RegisterCustomerCommand : IRequest<RegisteredResponse>
{
    public RegisterCustomerCommandRequest RegisterCustomer { get; set; }
    public string IpAdress { get; set; }

    public RegisterCustomerCommand()
    {
        RegisterCustomer = null!;
        IpAdress = string.Empty;
    }

    public RegisterCustomerCommand(RegisterCustomerCommandRequest registerCustomer, string ipAdress)
    {
        RegisterCustomer = registerCustomer;
        IpAdress = ipAdress;
    }

    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, RegisteredResponse>
    {
        private readonly IAuthService _authServise;
        public readonly ICustomerService _customerService;
        public readonly IUserOperationClaimRepository _userOperationClaimRepository;
        public readonly IMapper _mapper;

        public RegisterCustomerCommandHandler(IAuthService authServise, ICustomerService customerService, IUserOperationClaimRepository userOperationClaimRepository, IMapper mapper)
        {
            _authServise = authServise;
            _customerService = customerService;
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
        }

        public async Task<RegisteredResponse> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer customer = _mapper.Map<Customer>(request.RegisterCustomer);

            HashingHelper.CreatePasswordHash
                (
                request.RegisterCustomer.Password,
                passwordHash:out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
                );
            customer.PasswordSalt = passwordSalt;
            customer.PasswordHash = passwordHash;
            await _customerService.AddAsync( customer );
            
            UserOperationClaim[] userCliams =
            {
                new() { UserId = customer.Id, OperationClaimId = 100,IsActive=true },
                new() { UserId = customer.Id, OperationClaimId = 101,IsActive=true },
            };

            foreach (var claim in userCliams)
            {
                await _userOperationClaimRepository.AddAsync(claim);
            }

            AccessToken accessToken = await _authServise.CreateAccessToken(customer);
            BaseRefreshToken refreshToken = await _authServise.CreateRefreshToken(customer,request.IpAdress);
            BaseRefreshToken addedRefreshToken = await _authServise.AddRefreshToken(refreshToken);
            RegisteredResponse registeredResponse = new() { Message= AuthMessages.SendEmailForEmailActivate };

            return registeredResponse;
        }
    }
}
