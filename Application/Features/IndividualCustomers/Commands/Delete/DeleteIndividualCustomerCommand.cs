using Application.Features.Customers.Rules;
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

namespace Application.Features.IndividualCustomers.Commands.Delete;

public class DeleteIndividualCustomerCommand: IRequest<DeletedIndividualCustomerResponse>
{
    public DeleteIndividualCustomerRequest individualCustomerRequest { get; set; }
    public class DeleteIndividualCustomerCommandHandler : IRequestHandler<DeleteIndividualCustomerCommand, DeletedIndividualCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IndividualCustomerBusinessRules _rules;
        private readonly IIndividualCustomerService _service;

        public DeleteIndividualCustomerCommandHandler(IMapper mapper,IndividualCustomerBusinessRules rules, IIndividualCustomerService service)
        {
            _rules = rules;
            _mapper = mapper;
            _service = service;
        }

        public async Task<DeletedIndividualCustomerResponse> Handle(DeleteIndividualCustomerCommand request, CancellationToken cancellationToken)
        {
            await _rules.CustomerIdShouldExistWhenSelected(request.individualCustomerRequest.Id);
            IndividualCustomer individualCustomer = await _service.DeleteAsync(request.individualCustomerRequest,cancellationToken:cancellationToken);
            DeletedIndividualCustomerResponse response = _mapper.Map<DeletedIndividualCustomerResponse>(individualCustomer);

            return response;
        }
    }
}
