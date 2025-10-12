using Application.Features.OperationClaims.Commands.Create;
using Application.Features.OperationClaims.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationClaimController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> AddOperationCliam(CreateOperationClaimCommand request) 
        {
            CreatedOperationClaimCommandResponse response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
