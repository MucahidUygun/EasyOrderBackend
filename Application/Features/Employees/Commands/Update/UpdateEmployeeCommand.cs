using Application.Features.Employees.Dtos.Requests;
using Application.Features.Employees.Dtos.Responses;
using Application.Services.Employees;
using AutoMapper;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommand:IRequest<UpdatedEmplooyeResponse>
{
    public UpdateEmployeeReqeust UpdateEmployeeReqeust { get; set; }

    public UpdateEmployeeCommand(UpdateEmployeeReqeust updateEmployeeReqeust)
    {
        UpdateEmployeeReqeust = updateEmployeeReqeust;
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, UpdatedEmplooyeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public UpdateEmployeeCommandHandler(IMapper mapper, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<UpdatedEmplooyeResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.UpdateEmployeeReqeust.Id is null)
                throw new BusinessException("Empoyee not exists");
            Employee employee = await _employeeService.UpdateAsync(request.UpdateEmployeeReqeust);

            UpdatedEmplooyeResponse response = _mapper.Map<UpdatedEmplooyeResponse>(employee);

            return response;
        }
    }
}
