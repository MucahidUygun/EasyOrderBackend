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

namespace Application.Features.Customers.Queries;

public class GetByIdCustomerQuery : IRequest<GetByIdCustomerQueryResponse>
{
    public GetByIdCustomerQueryRequest request { get; set; }

    public class GetByIdCustomerHandle : IRequestHandler<GetByIdCustomerQuery, GetByIdCustomerQueryResponse>
    {
        public readonly IMapper _mapper;
        public readonly CustomerBusinessRules _rules;
        public readonly ICustomerService _service;

        public GetByIdCustomerHandle(IMapper mapper, CustomerBusinessRules rules, ICustomerService service)
        {
            _rules = rules;
            _mapper = mapper;
            _service = service;
        }

        public async Task<GetByIdCustomerQueryResponse> Handle(GetByIdCustomerQuery request, CancellationToken cancellationToken)
        {
            await _rules.CustomerIdShouldExistWhenSelected(request.request.Id, cancellationToken: cancellationToken);
            Customer customer = await _service.GetAsync(p => p.Id == request.request.Id);

            GetByIdCustomerQueryResponse response = _mapper.Map<GetByIdCustomerQueryResponse>(customer);

            return response;
        }
    }
}
