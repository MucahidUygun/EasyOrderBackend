using Application.Features.CorporateCustomers.Dtos.Responses;
using Application.Features.CorporateCustomers.Rules;
using Application.Services.CorporateCustomers;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Queries;

public class GetListCorporateCustomerQuery : IRequest<IPaginate<GetListCorporateCustomerQueryResponse>>
{
    public PaginationParams pageRequest { get; set; }
    public class GetListCorporateCustomerQueryHandler : IRequestHandler<GetListCorporateCustomerQuery, IPaginate<GetListCorporateCustomerQueryResponse>>
    {
        private readonly IMapper _mapper;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly CorporateCustomerBusinessRules _corporateCustomerBusinessRules;

        public GetListCorporateCustomerQueryHandler(IMapper mapper, ICorporateCustomerService corporateCustomerService, CorporateCustomerBusinessRules corporateCustomerBusinessRules)
        {
            _mapper = mapper;
            _corporateCustomerService = corporateCustomerService;
            _corporateCustomerBusinessRules = corporateCustomerBusinessRules;
        }

        public async Task<IPaginate<GetListCorporateCustomerQueryResponse>> Handle(GetListCorporateCustomerQuery request, CancellationToken cancellationToken)
        {
            IPaginate<CorporateCustomer>? corporateCustomers = await _corporateCustomerService.GetAllAsync(
                index:request.pageRequest.PageNumber,
                size:request.pageRequest.PageSize,
                cancellationToken:cancellationToken);
            Paginate<GetListCorporateCustomerQueryResponse> responses = _mapper.Map<Paginate<GetListCorporateCustomerQueryResponse>>(corporateCustomers);
            return responses;
        }
    }
}
