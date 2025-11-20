using Application.Features.Employees.Dtos.Responses;
using Application.Services.Employees;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Queries;

public class GetListEmployeeQuery:IRequest<IPaginate<GetListEmplooyeResponse>>
{
    public PaginationParams PaginationParams { get; set; }
    public class GetListEmployeeQueryHandler : IRequestHandler<GetListEmployeeQuery, IPaginate<GetListEmplooyeResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public GetListEmployeeQueryHandler(IMapper mapper, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<IPaginate<GetListEmplooyeResponse>> Handle(GetListEmployeeQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Employee> employees = await _employeeService.GetAllAsync(
                size:request.PaginationParams.PageSize,
                index:request.PaginationParams.PageNumber,
                cancellationToken:cancellationToken);
            Paginate<GetListEmplooyeResponse> responses = _mapper.Map<Paginate<GetListEmplooyeResponse>>(employees);
            return responses;
        }
    }
}
