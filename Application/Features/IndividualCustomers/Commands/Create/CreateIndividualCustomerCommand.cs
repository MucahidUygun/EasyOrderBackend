using Application.Features.IndividualCustomers.Dtos.Requests;
using Application.Features.IndividualCustomers.Dtos.Responses;
using Application.Services.IndividualCustomers;
using AutoMapper;
using Core.Security.Hashing;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.IndividualCustomers.Commands.Create;

public class CreateIndividualCustomerCommand : IRequest<CreatedIndividualCustomerResponse>
{
    public CreateIndividualCustomerRequest individualCustomerRequest { get; set; }

    public class CreateIndividualCustomerCommandHandler : IRequestHandler<CreateIndividualCustomerCommand, CreatedIndividualCustomerResponse>
    {
        private readonly IMapper _mapper;
        private readonly IIndividualCustomerService _service;

        public CreateIndividualCustomerCommandHandler(IMapper mapper, IIndividualCustomerService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public async Task<CreatedIndividualCustomerResponse> Handle(CreateIndividualCustomerCommand request, CancellationToken cancellationToken)
        {
            IndividualCustomer individualCustomer = _mapper.Map<IndividualCustomer>(request.individualCustomerRequest);
            HashingHelper.CreatePasswordHash
                (
                request.individualCustomerRequest.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
                );
            individualCustomer.PasswordHash = passwordHash;
            individualCustomer.PasswordSalt = passwordSalt;

             await _service.AddAsync(individualCustomer,cancellationToken);

            CreatedIndividualCustomerResponse addedIndividualCustomer = _mapper.Map<CreatedIndividualCustomerResponse>(individualCustomer);

            return addedIndividualCustomer;
        }
    }
}
