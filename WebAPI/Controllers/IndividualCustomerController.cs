using Application.Features.Customers.Dtos.Responses;
using Application.Features.Customers.Queries;
using Application.Features.IndividualCustomers.Commands.Create;
using Application.Features.IndividualCustomers.Commands.Delete;
using Application.Features.IndividualCustomers.Commands.Update;
using Application.Features.IndividualCustomers.Dtos.Responses;
using Application.Features.IndividualCustomers.Queries;
using Core.Persistence.Paging;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IndividualCustomerController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateIndividualCustomerCommand request)
    {
        CreatedIndividualCustomerResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateIndividualCustomerCommand request)
    {
        UpdatedIndividualCustomerResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteIndividualCustomerCommand request)
    {
        DeletedIndividualCustomerResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpGet("GetById")]
    public async Task<IActionResult> GetById([FromQuery] GetByIdIndividualCustomerQuery request)
    {
        GetByIdIndividualCustomerQueryResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpGet("GetList")]
    public async Task<IActionResult> GetList([FromQuery] PaginationParams request)
    {
        GetListIndividualCustomerQuery query = new() { pageRequest=request };
        IPaginate<GetListIndividualCustomerQueryResponse> response = await Mediator.Send(query);
        return Ok(response);
    }
}
