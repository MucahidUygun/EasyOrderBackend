using Application.Features.CorporateCustomers.Dtos.Requests;
using Application.Features.CorporateCustomers.Dtos.Responses;
using Application.Features.CorporateCustomers.Rules;
using Application.Services.CorporateCustomers;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Security.Hashing;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Commands.Create;

public class CreateCorporateCustomerCommand : IRequest<CreatedCorporateCustomerResponse>,ISecuredRequest
{
    public CreateCorporateCustomerRequest corporateCustomerRequest { get; set; }

    public string[] Claims => ["Admin", "Satış Temsilcisi"];

    public class CreateCorporateCustomerHandler : IRequestHandler<CreateCorporateCustomerCommand, CreatedCorporateCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly CorporateCustomerBusinessRules _rules;

        public CreateCorporateCustomerHandler(IMapper mapper, ICorporateCustomerService corporateCustomerService, CorporateCustomerBusinessRules rules)
        {
            _mapper = mapper;
            _corporateCustomerService = corporateCustomerService;
            _rules = rules;
        }

        public async Task<CreatedCorporateCustomerResponse> Handle(CreateCorporateCustomerCommand request, CancellationToken cancellationToken)
        {
            CorporateCustomer corporateCustomer = _mapper.Map<CorporateCustomer>(request.corporateCustomerRequest);

            HashingHelper.CreatePasswordHash
                (
                password:request.corporateCustomerRequest.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
                );

            corporateCustomer.PasswordHash = passwordHash;
            corporateCustomer.PasswordSalt = passwordSalt;

             await _corporateCustomerService.AddAsync(corporateCustomer,cancellationToken);

            CreatedCorporateCustomerResponse response = _mapper.Map<CreatedCorporateCustomerResponse>(corporateCustomer);


            return response;
        }
    }
}
