using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.Registers.RegisterCustomer;
using Application.Features.Auth.Dtos.Requests;
using Application.Features.Auth.Dtos.Responses;
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
            setRefreshTokenToCookie(response.RefreshToken);
            return Created(uri: "", response.AccessToken);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommandRequest loginCommandRequest)
        {
            LoginCommand loginCommand = new() { LoginCustomerCommandRequest = loginCommandRequest , IpAdress = getIpAddress()};
            LoggedResponse response = await Mediator.Send(loginCommand);
            setRefreshTokenToCookie(response.RefreshToken);
            return Ok(response.AccessToken);
        }

        private void setRefreshTokenToCookie(RefreshToken refreshToken)
        {
            CookieOptions cookieOptions = new() { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(7) };
            Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
