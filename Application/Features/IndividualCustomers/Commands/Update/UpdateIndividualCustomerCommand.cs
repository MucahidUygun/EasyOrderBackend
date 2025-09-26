using Application.Features.IndividualCustomers.Dtos.Requests;
using Application.Features.IndividualCustomers.Dtos.Responses;
using Application.Features.IndividualCustomers.Rules;
using Application.Services.IndividualCustomers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.IndividualCustomers.Commands.Update;

public class UpdateIndividualCustomerCommand: IRequest<UpdatedIndividualCustomerResponse>  
{
    public UpdateIndividualCustomerRequest individualCustomerRequest { get; set; }

    public class UpdateIndividualCustomerCommandHandler : IRequestHandler<UpdateIndividualCustomerCommand, UpdatedIndividualCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IndividualCustomerBusinessRules _rules;
        private readonly IIndividualCustomerService _service;

        public UpdateIndividualCustomerCommandHandler(IMapper mapper,IndividualCustomerBusinessRules rules, IIndividualCustomerService service)
        {
            _rules = rules;
            _mapper = mapper;
            _service = service;
        }

        public async Task<UpdatedIndividualCustomerResponse> Handle(UpdateIndividualCustomerCommand request, CancellationToken cancellationToken)
        {
            await _rules.CustomerIdShouldExistWhenSelected(request.individualCustomerRequest.Id);
            IndividualCustomer customer = await _service.UpdateAsync(request.individualCustomerRequest,cancellationToken);

            UpdatedIndividualCustomerResponse response = _mapper.Map<UpdatedIndividualCustomerResponse>(customer);

            return response;

        }
    }
}
