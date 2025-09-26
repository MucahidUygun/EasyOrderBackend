using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Responses;
using Application.Features.Customers.Rules;
using Application.Services.Customers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommand : IRequest<DeletedCustomerResponse>
{
    public DeleteCustomerRequest request { get; set; }

    class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, DeletedCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerService _service;
        private readonly CustomerBusinessRules _rules;

        public DeleteCustomerHandler(IMapper mapper, ICustomerService service,CustomerBusinessRules rules)
        {
            _rules = rules;
            _mapper = mapper;
            _service = service;
        }

        public async Task<DeletedCustomerResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            await _rules.CustomerIdShouldExistWhenSelected(request.request.Id,cancellationToken:cancellationToken);
            DeletedCustomerResponse response = _mapper.Map<DeletedCustomerResponse>
                (
                await _service.DeleteAsync(request.request)
                );

            return response;

        }
    }
}
