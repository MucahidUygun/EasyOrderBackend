using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using AutoMapper;
using Core.Entities;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<LoggedResponse>
{
    public LoginCommandRequest? LoginCustomerCommandRequest { get; set; }
    public string IpAdress { get; set; }

    public LoginCommand(LoginCommandRequest? loginCustomerCommandRequest, string ıpAdress)
    {
        LoginCustomerCommandRequest = loginCustomerCommandRequest;
        IpAdress = ıpAdress;
    }
    public LoginCommand()
    {
        LoginCustomerCommandRequest = null;
        IpAdress = string.Empty;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedResponse>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;

        public LoginCommandHandler(IMapper mapper, IAuthService authService, AuthBusinessRules authBusinessRules)
        {
            _mapper = mapper;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<LoggedResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _authBusinessRules.LoginRequestIsNull(request.LoginCustomerCommandRequest);
            User? user = await _authService.GetUserAsync(predicate: p=>p.Email == request.LoginCustomerCommandRequest!.Email);
            _authBusinessRules.CheckUserIsNull(user,AuthMessages.EmailCannotBeEmpty);
            LoggedResponse loggedResponse = new LoggedResponse();

            _authBusinessRules.UserShouldBeExistsWhenSelected(user);
            _authBusinessRules.UserPasswordShouldBeMatch(user!,request.LoginCustomerCommandRequest!.Password);

            AccessToken accessToken = await _authService.CreateAccessToken(user);
            BaseRefreshToken refreshToken = await _authService.CreateRefreshToken(user,ipAdress:request.IpAdress);
            await _authService.DeleteOldRefreshToken(user,request.IpAdress);
            BaseRefreshToken addedRefreshToken = await _authService.AddRefreshToken(refreshToken);

            loggedResponse.AccessToken = accessToken;
            loggedResponse.RefreshToken = addedRefreshToken;

            return loggedResponse;
        }
    }
}
