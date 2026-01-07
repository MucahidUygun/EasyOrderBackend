using Application.Features.CorporateCustomers.Dtos.Requests;
using Application.Features.CorporateCustomers.Dtos.Responses;
using Application.Features.CorporateCustomers.Rules;
using Application.Services.CorporateCustomers;
using AutoMapper;
using Core.Application.Pipelines.Logging;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Commands.Update;

public class UpdateCorporateCustomerCommand : IRequest<UpdatedCorporateCustomerResponse>, ILoggableRequest
{
    public UpdateCorporateCustomerRequest updateCorporateCustomerRequest {  get; set; }

    public class UpdateCorporateCustomerCommandHandler : IRequestHandler<UpdateCorporateCustomerCommand, UpdatedCorporateCustomerResponse>
    {
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly IMapper _mapper;
        private readonly CorporateCustomerBusinessRules _rules;

        public UpdateCorporateCustomerCommandHandler(ICorporateCustomerService corporateCustomerService, IMapper mapper, CorporateCustomerBusinessRules rules)
        {
            _corporateCustomerService = corporateCustomerService;
            _mapper = mapper;
            _rules = rules;
        }

        public async Task<UpdatedCorporateCustomerResponse> Handle(UpdateCorporateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _rules.CorporateCustomerIdShouldExistWhenSelected(request.updateCorporateCustomerRequest.Id);
            CorporateCustomer updatedCorporateCustomer = await _corporateCustomerService.UpdateAsync(request.updateCorporateCustomerRequest,cancellationToken);

            UpdatedCorporateCustomerResponse response = _mapper.Map<UpdatedCorporateCustomerResponse>(updatedCorporateCustomer);

            return response;
        }
    }
}
