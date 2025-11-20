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

namespace Application.Features.Employees.Commands.Delete;

public class DeleteEmployeeCommand:IRequest<DeletedEmplooyeResponse>
{
    public DeleteEmployeeReqeust DeleteEmployeeReqeust { get; set; }
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, DeletedEmplooyeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeeCommandHandler(IMapper mapper, IEmployeeService employeeService)
        {
            _mapper = mapper;
            _employeeService = employeeService;
        }

        public async Task<DeletedEmplooyeResponse> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.DeleteEmployeeReqeust.Id is null)
                throw new BusinessException("Employee not exists");
            Employee deletedEmployee = await _employeeService.DeleteAsync(request.DeleteEmployeeReqeust);
            DeletedEmplooyeResponse deletedEmplooyeResponse = _mapper.Map<DeletedEmplooyeResponse>(deletedEmployee);
            return deletedEmplooyeResponse;
        }
    }
}
