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

namespace Application.Features.Customers.Commands.Update;

public class UpdateCustomerCommand : IRequest<UpdatedCustomerResponse>
{
    public UpdateCustomerRequest request { get; set; }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdatedCustomerResponse>
    {
        public readonly IMapper _mapper;
        public readonly CustomerBusinessRules _rules;
        public readonly ICustomerService _service;

        public UpdateCustomerCommandHandler(IMapper mapper, ICustomerService service,CustomerBusinessRules rules)
        {
            _rules = rules;
            _mapper = mapper;
            _service = service;
        }

        public async Task<UpdatedCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _rules.CustomerIdShouldExistWhenSelected(request.request.Id, cancellationToken: cancellationToken);
            Customer updatedCustomer = await _service.UpdateAsync(request.request, cancellationToken);

            UpdatedCustomerResponse response = _mapper.Map<UpdatedCustomerResponse>(updatedCustomer);

            return response;
        }
    }
}
