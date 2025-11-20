using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Services.AuthService;
using Application.Services.Employees;
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

namespace Application.Features.Auth.Commands.Registers.RegisterEmployee;

public class RegisterEmployeeCommand : IRequest<RegisteredResponse>
{
    public RegisterEmployeeCommandRequest? registerEmployeeRequest { get; set; }
    public string IpAdress {  get; set; }

    public RegisterEmployeeCommand()
    {
        registerEmployeeRequest = null;
        IpAdress = string.Empty;
    }

    public RegisterEmployeeCommand(RegisterEmployeeCommandRequest request, string ipAdress)
    {
        this.registerEmployeeRequest = request;
        IpAdress = ipAdress;
    }
    public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, RegisteredResponse>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IEmployeeService _employeeService;

        public RegisterEmployeeCommandHandler(IMapper mapper, IAuthService authService, IUserOperationClaimRepository userOperationClaimRepository, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _authService = authService;
            _userOperationClaimRepository = userOperationClaimRepository;
            _employeeService = employeeService;
        }

        public async Task<RegisteredResponse> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
        {
            Employee employee = _mapper.Map<Employee>(request.registerEmployeeRequest);
            HashingHelper.CreatePasswordHash
                (
                password: request.registerEmployeeRequest.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
                );
            employee.PasswordHash = passwordHash;
            employee.PasswordSalt = passwordSalt;
            Employee addedEmployee = await _employeeService.AddAsync(employee);
            UserOperationClaim[] userCliams =
            {
                new() { UserId = addedEmployee.Id, OperationClaimId = 100,IsActive=true },
                new() { UserId = addedEmployee.Id, OperationClaimId = 101,IsActive=true },
            };

            foreach (var claim in userCliams)
            {
                await _userOperationClaimRepository.AddAsync(claim);
            }

            AccessToken accessToken = await _authService.CreateAccessToken(addedEmployee);
            BaseRefreshToken refreshToken = await _authService.CreateRefreshToken(addedEmployee, request.IpAdress);
            BaseRefreshToken addedRefreshToken = await _authService.AddRefreshToken(refreshToken);

            RegisteredResponse response = new() { AccessToken = accessToken,RefreshToken = addedRefreshToken };
            return response;
        }
    }
}
