using Application.Features.Employees.Dtos.Requests;
using Application.Features.Employees.Dtos.Responses;
using Application.Services.Employees;
using AutoMapper;
using Core.Security.Hashing;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommand:IRequest<CreatedEmplooyeResponse>
{
    public CreateEmplooyeRequest CreateEmplooyeRequest { get; set; }

    public CreateEmployeeCommand(CreateEmplooyeRequest createEmplooyeRequest)
    {
        CreateEmplooyeRequest = createEmplooyeRequest;
    }

    public class CreateEmpoyeeCommandHandler : IRequestHandler<CreateEmployeeCommand, CreatedEmplooyeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public CreateEmpoyeeCommandHandler(IMapper mapper, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<CreatedEmplooyeResponse> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            Employee employee = _mapper.Map<Employee>(request.CreateEmplooyeRequest);
            HashingHelper.CreatePasswordHash
                (
                password:request.CreateEmplooyeRequest.Password,
                passwordHash:out byte[] passwordHash,
                passwordSalt:out byte[] passwordSalt
                );
            employee.PasswordHash = passwordHash;
            employee.PasswordSalt = passwordSalt;
            Employee addedEmployee = await _employeeService.AddAsync(employee);
            CreatedEmplooyeResponse createdEmplooyeResponse = _mapper.Map<CreatedEmplooyeResponse>(addedEmployee);
            return createdEmplooyeResponse;
        }
    }
}
