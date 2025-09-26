using Application.Features.IndividualCustomers.Dtos.Responses;
using Application.Services.IndividualCustomers;
using AutoMapper;
using Core.Application.Requests;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.IndividualCustomers.Queries;

public class GetListIndividualCustomerQuery: IRequest<IPaginate<GetListIndividualCustomerQueryResponse>>
{
    public PaginationParams pageRequest { get; set; }

    public class GetListIndividualCustomerQueryHandler : IRequestHandler<GetListIndividualCustomerQuery, IPaginate<GetListIndividualCustomerQueryResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IIndividualCustomerService _service;

        public GetListIndividualCustomerQueryHandler(IMapper mapper, IIndividualCustomerService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<IPaginate<GetListIndividualCustomerQueryResponse>> Handle(GetListIndividualCustomerQuery request, CancellationToken cancellationToken)
        {
            IPaginate<IndividualCustomer>? individualCustomers = await _service.GetAllAsync(index:request.pageRequest.PageNumber,size:request.pageRequest.PageSize,cancellationToken:cancellationToken);

            Paginate<GetListIndividualCustomerQueryResponse> responses = _mapper.Map<Paginate<GetListIndividualCustomerQueryResponse>>(individualCustomers);

            return responses;
        }
    }
}
