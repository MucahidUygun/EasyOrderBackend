using Application.Features.Customers.Queries;
using Application.Features.Employees.Commands.Create;
using Application.Features.Employees.Commands.Delete;
using Application.Features.Employees.Commands.Update;
using Application.Features.Employees.Dtos.Requests;
using Application.Features.Employees.Dtos.Responses;
using Application.Features.Employees.Queries;
using Azure.Core;
using Core.Persistence.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateEmployeeCommand reqeust)
    {
        CreatedEmplooyeResponse response = await Mediator.Send(reqeust);
        return Ok(response);
    }
    [HttpDelete("id")]
    public async Task<IActionResult> Delete([FromBody] Guid id)
    {
        DeleteEmployeeReqeust reqeust = new() { Id = id };
        DeleteEmployeeCommand command = new() { DeleteEmployeeReqeust = reqeust };
        DeletedEmplooyeResponse response = await Mediator.Send(command);
        return Ok(response);
    }
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateEmployeeCommand reqeust)
    {
        UpdatedEmplooyeResponse response = await Mediator.Send(reqeust);
        return Ok(response);
    }
    [HttpGet("GetById")]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        GetByIdEmplooyeRequest request = new() { Id = id };
        GetByIdEmployeeQuery query = new() { emplooyeRequest = request };
        GetByIdEmplooyeResponse response = await Mediator.Send(query);
        return Ok(response);
    }
    [HttpGet("GetList")]
    public async Task<IActionResult> GetList([FromQuery]PaginationParams request)
    {
        GetListEmployeeQuery query = new() { PaginationParams = request };
        IPaginate<GetListEmplooyeResponse> response = await Mediator.Send(query);
        return Ok(response);
    }
}
