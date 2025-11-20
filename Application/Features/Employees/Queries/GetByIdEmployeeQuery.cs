using Application.Features.Employees.Dtos.Requests;
using Application.Features.Employees.Dtos.Responses;
using Application.Services.Employees;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Queries;

public class GetByIdEmployeeQuery:IRequest<GetByIdEmplooyeResponse>
{
    public GetByIdEmplooyeRequest emplooyeRequest { get; set; }

    public class GetByIdEmployeeQueryHandler : IRequestHandler<GetByIdEmployeeQuery, GetByIdEmplooyeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public GetByIdEmployeeQueryHandler(IMapper mapper, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<GetByIdEmplooyeResponse> Handle(GetByIdEmployeeQuery request, CancellationToken cancellationToken)
        {
            Employee employee = await _employeeService.GetAsync(p=>p.Id==request.emplooyeRequest.Id);
            GetByIdEmplooyeResponse response = _mapper.Map<GetByIdEmplooyeResponse>(employee);
            return response;
        }
    }
}
