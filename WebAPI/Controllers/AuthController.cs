using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.Registers.RegisterCustomer;
using Application.Features.Auth.Commands.Registers.RegisterCustomers;
using Application.Features.Auth.Commands.Registers.RegisterEmployee;
using Application.Features.Auth.Commands.VerifyAccount;
using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
using Core.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerCommandRequest request) 
        {
            RegisterCustomerCommand registerCustomer = new() { IpAdress=getIpAddress(),RegisterCustomer = request };
            RegisteredResponse response = await Mediator.Send(registerCustomer);
            //setRefreshTokenToCookie(response.RefreshToken);
            return Created(uri: "", response.Message);
        }

        [HttpPost("RegisterCorporateCustomer")]
        public async Task<IActionResult> RegisterCorporateCustomer([FromBody] RegisterCorporateCustomerRequest request)
        {
            RegisterCorporateCustomerCommand registerCustomer = new() { IpAdress = getIpAddress(), RegisterCorporateCustomerRequests = request };
            RegisteredResponse response = await Mediator.Send(registerCustomer);
            //setRefreshTokenToCookie(response.RefreshToken);
            return Created(uri: "", response.Message);
        }
        [HttpPost("RegisterIndividualCustomer")]
        public async Task<IActionResult> RegisterIndividualCustomer([FromBody] RegisterIndiviualCustomerCommandRequest request)
        {
            RegisterIndividualCustomerCommand registerCustomer = new() { IpAdress = getIpAddress(), CommandRequest = request };
            RegisteredResponse response = await Mediator.Send(registerCustomer);
            //setRefreshTokenToCookie(response.RefreshToken);
            return Created(uri: "", response.Message);
        }
        [HttpPost("RegisterEmployee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeCommandRequest request)
        {
            RegisterEmployeeCommand registerEmployee = new() { registerEmployeeRequest = request, IpAdress = getIpAddress() };
            RegisteredResponse response = await Mediator.Send(registerEmployee);
            //setRefreshTokenToCookie(response.RefreshToken);
            return Created(uri: "", response.Message);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommandRequest loginCommandRequest)
        {
            LoginCommand loginCommand = new() { LoginCustomerCommandRequest = loginCommandRequest , IpAdress = getIpAddress()};
            LoggedResponse response = await Mediator.Send(loginCommand);
            setRefreshTokenToCookie(response.RefreshToken);
            return Ok(response.AccessToken);
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            ExitedResponse response = await Mediator.Send(new LogoutCommand());
            return Ok(response);
        }
        [HttpGet("VerifyAccount")]
        public async Task<IActionResult> VerifyAccount([FromQuery] VerifyEmailCommandRequest commandRequest)
        {
            VerifyAccountCommand command = new() { VerifyEmailCommandRequest = commandRequest };
            await Mediator.Send(command);
            return Ok();
        }

        private void setRefreshTokenToCookie(BaseRefreshToken refreshToken)
        {
            CookieOptions cookieOptions = new() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
            Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
