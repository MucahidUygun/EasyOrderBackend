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

namespace Application.Features.Customers.Queries;

public class GetByIdCustomerQuery : IRequest<GetByIdCustomerQueryResponse>
{
    public GetByIdCustomerQueryRequest request { get; set; }

    public class GetByIdCustomerHandle : IRequestHandler<GetByIdCustomerQuery, GetByIdCustomerQueryResponse>
    {
        public readonly IMapper _mapper;
        public readonly ICustomerService _service;

        public GetByIdCustomerHandle(IMapper mapper, ICustomerService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<GetByIdCustomerQueryResponse> Handle(GetByIdCustomerQuery request, CancellationToken cancellationToken)
        {
            Customer customer = await _service.GetAsync(p=>p.Id == request.request.Id);

            GetByIdCustomerQueryResponse response = _mapper.Map<GetByIdCustomerQueryResponse>(customer);

            return response;
        }
    }
}
