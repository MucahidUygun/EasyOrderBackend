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

namespace Application.Features.Customers.Commands.Delete;

public class DeleteCustomerCommand : IRequest<DeletedCustomerResponse>
{
    public DeleteCustomerRequest request { get; set; }

    class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, DeletedCustomerResponse>
    {
        public readonly IMapper _mapper;
        public readonly ICustomerService _service;

        public DeleteCustomerHandler(IMapper mapper, ICustomerService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<DeletedCustomerResponse> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            DeletedCustomerResponse response = _mapper.Map<DeletedCustomerResponse>
                (
                await _service.DeleteAsync(request.request)
                );

            return response;

        }
    }
}
