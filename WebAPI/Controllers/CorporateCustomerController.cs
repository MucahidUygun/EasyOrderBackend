using Application.Features.CorporateCustomers.Commands.Create;
using Application.Features.CorporateCustomers.Commands.Delete;
using Application.Features.CorporateCustomers.Commands.Update;
using Application.Features.CorporateCustomers.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorporateCustomerController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateCorporateCustomerCommand request)
        {
            CreatedCorporateCustomerResponse response = await Mediator.Send(request);
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCorporateCustomerCommand request) 
        {
            UpdatedCorporateCustomerResponse response = await Mediator.Send(request);
            return Ok(response);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCorporateCustomerCommand request)
        {
            DeletedCorporateCustomerResponse response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
