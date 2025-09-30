using Application.Features.CorporateCustomers.Dtos.Requests;
using Application.Features.CorporateCustomers.Dtos.Responses;
using Application.Features.CorporateCustomers.Rules;
using Application.Services.CorporateCustomers;
using AutoMapper;
using Core.Security.Hashing;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Queries;

public class GetByIdCorporateCustomerQuery : IRequest<GetByIdCorporateCustomerQueryResponse>
{
    public GetByIdCorporateCustomerRequest getByIdCorporateCustomerRequest { get; set; }

    public class GetByIdCorporateCustomerQueryHandler : IRequestHandler<GetByIdCorporateCustomerQuery, GetByIdCorporateCustomerQueryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly CorporateCustomerBusinessRules _corporateCustomerBusinessRules;

        public GetByIdCorporateCustomerQueryHandler(IMapper mapper, ICorporateCustomerService corporateCustomerService, CorporateCustomerBusinessRules corporateCustomerBusinessRules)
        {
            _mapper = mapper;
            _corporateCustomerService = corporateCustomerService;
            _corporateCustomerBusinessRules = corporateCustomerBusinessRules;
        }

        public async Task<GetByIdCorporateCustomerQueryResponse> Handle(GetByIdCorporateCustomerQuery request, CancellationToken cancellationToken)
        {
            await _corporateCustomerBusinessRules.CorporateCustomerIdShouldExistWhenSelected(request.getByIdCorporateCustomerRequest.Id);
            CorporateCustomer corporateCustomer = await _corporateCustomerService.GetAsync(p=>p.Id==request.getByIdCorporateCustomerRequest.Id,cancellationToken:cancellationToken);
            GetByIdCorporateCustomerQueryResponse response = _mapper.Map<GetByIdCorporateCustomerQueryResponse>(corporateCustomer);
            return response;
        }
    }
}
