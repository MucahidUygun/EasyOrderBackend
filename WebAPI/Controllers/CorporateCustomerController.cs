using Application.Features.CorporateCustomers.Commands.Create;
using Application.Features.CorporateCustomers.Commands.Delete;
using Application.Features.CorporateCustomers.Commands.Update;
using Application.Features.CorporateCustomers.Dtos.Responses;
using Application.Features.CorporateCustomers.Queries;
using Core.Persistence.Paging;
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
        [HttpDelete("Id")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            DeleteCorporateCustomerCommand request = new()
            {
                deleteCorporateCustomerRequest = new() { Id = id }
            };
            DeletedCorporateCustomerResponse response = await Mediator.Send(request);
            return Ok(response);
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] GetByIdCorporateCustomerQuery request)
        {
            GetByIdCorporateCustomerQueryResponse response = await Mediator.Send(request);
            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            GetListCorporateCustomerQuery query = new (){pageRequest=paginationParams };
            IPaginate<GetListCorporateCustomerQueryResponse> responses = await Mediator.Send(query);
            return Ok(responses);
        }
    }
}
