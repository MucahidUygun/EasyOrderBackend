using Application.Features.Customers.Dtos.Responses;
using Application.Services.Customers;
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

namespace Application.Features.Customers.Queries;

public class GetListCustomerQuery : IRequest<IPaginate<GetListCustomerQueryResponse>>
{
    public PaginationParams pageRequest { get; set; }

    public class GetListCustomerQueryHandler : IRequestHandler<GetListCustomerQuery, IPaginate<GetListCustomerQueryResponse>>
    {
        public readonly IMapper _mapper;
        public readonly ICustomerService _service;

        public GetListCustomerQueryHandler(IMapper mapper, ICustomerService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<IPaginate<GetListCustomerQueryResponse>> Handle(GetListCustomerQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Customer>? customers = await _service.GetAllAsync(index: request.pageRequest.PageNumber,size: request.pageRequest.PageSize);

            Paginate<GetListCustomerQueryResponse> responses = _mapper.Map<Paginate<GetListCustomerQueryResponse>>(customers);

            return responses;
        }
    }
}
