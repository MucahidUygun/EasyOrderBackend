using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Responses;
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
        public readonly ICustomerService _service;

        public UpdateCustomerCommandHandler(IMapper mapper, ICustomerService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<UpdatedCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer updatedCustomer = await _service.UpdateAsync(request.request, cancellationToken);

            UpdatedCustomerResponse response = _mapper.Map<UpdatedCustomerResponse>(updatedCustomer);

            return response;
        }
    }
}
