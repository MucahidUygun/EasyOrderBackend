using Application.Features.Customers.Dtos.Responses;
using Application.Features.IndividualCustomers.Dtos.Requests;
using Application.Features.IndividualCustomers.Dtos.Responses;
using Application.Features.IndividualCustomers.Rules;
using Application.Services.IndividualCustomers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IndividualCustomers.Queries;

public class GetByIdIndividualCustomerQuery : IRequest<GetByIdIndividualCustomerQueryResponse>
{
    public GetByIdIndividualCustomerQueryRequest IndividualCustomerQueryRequest { get; set; }

    public class GetByIdIndividualCustomerQueryHandler : IRequestHandler<GetByIdIndividualCustomerQuery, GetByIdIndividualCustomerQueryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IIndividualCustomerService _service;
        private readonly IndividualCustomerBusinessRules _rules;

        public GetByIdIndividualCustomerQueryHandler(IMapper mapper, IIndividualCustomerService service,IndividualCustomerBusinessRules rules)
        {
            _rules = rules;
            _mapper = mapper;
            _service = service;
        }

        public async Task<GetByIdIndividualCustomerQueryResponse> Handle(GetByIdIndividualCustomerQuery request, CancellationToken cancellationToken)
        {
            await _rules.CustomerIdShouldExistWhenSelected(request.IndividualCustomerQueryRequest.Id);
            IndividualCustomer individualCustomer = await _service.GetAsync(p=>p.Id==request.IndividualCustomerQueryRequest.Id);

            GetByIdIndividualCustomerQueryResponse response = _mapper.Map<GetByIdIndividualCustomerQueryResponse>(individualCustomer);
            return response;
        }
    }
}