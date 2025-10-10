using Application.Features.Customers.Commands.Create;
using Application.Features.Customers.Commands.Delete;
using Application.Features.Customers.Commands.Update;
using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Response;
using Application.Features.Customers.Dtos.Responses;
using Application.Features.Customers.Queries;
using Core.Persistence.Paging;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateCustomerCommand request)
    {
        CreatedCustomerResponse response = await Mediator.Send(request);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerCommand request)
    {
        UpdatedCustomerResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCustomerCommand request)
    {
        DeletedCustomerResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpGet("GetById")]
    public async Task<IActionResult> GetById([FromQuery] GetByIdCustomerQuery request)
    {
        GetByIdCustomerQueryResponse response = await Mediator.Send(request);
        return Ok(response);
    }
    [HttpGet("GetList")]
    public async Task<IActionResult> GetList([FromQuery] PaginationParams request)
    {
        GetListCustomerQuery query = new() { pageRequest = request };
        IPaginate<GetListCustomerQueryResponse> response = await Mediator.Send(query);
        return Ok(response);
    }
}
