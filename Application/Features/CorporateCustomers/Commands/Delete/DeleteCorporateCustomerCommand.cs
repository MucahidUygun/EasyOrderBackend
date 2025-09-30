using Application.Features.CorporateCustomers.Dtos.Requests;
using Application.Features.CorporateCustomers.Dtos.Responses;
using Application.Features.CorporateCustomers.Rules;
using Application.Services.CorporateCustomers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Commands.Delete;

public class DeleteCorporateCustomerCommand : IRequest<DeletedCorporateCustomerResponse>
{
    public DeleteCorporateCustomerRequest deleteCorporateCustomerRequest { get; set; }

    public class DeleteCorporateCustomerCommandCommandHandler : IRequestHandler<DeleteCorporateCustomerCommand, DeletedCorporateCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly CorporateCustomerBusinessRules _rules;

        public DeleteCorporateCustomerCommandCommandHandler(IMapper mapper, ICorporateCustomerService corporateCustomerService, CorporateCustomerBusinessRules rules)
        {
            _mapper = mapper;
            _corporateCustomerService = corporateCustomerService;
            _rules = rules;
        }

        public async Task<DeletedCorporateCustomerResponse> Handle(DeleteCorporateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _rules.CorporateCustomerIdShouldExistWhenSelected(request.deleteCorporateCustomerRequest.Id);
            CorporateCustomer deletedCorporateCustomer = await _corporateCustomerService.DeleteAsync(request.deleteCorporateCustomerRequest, cancellationToken);

            DeletedCorporateCustomerResponse response = _mapper.Map<DeletedCorporateCustomerResponse>(deletedCorporateCustomer);

            return response;
        }
    }
}
