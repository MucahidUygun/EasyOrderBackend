using AutoMapper;
using MediatR;
using Application.Features.Customers.Dtos.Requests;
using Domain.Entities;
using Application.Features.Customers.Dtos.Response;
using Application.Services.Customers;
using Core.Security.Hashing;

namespace Application.Features.Customers.Commands.Create;

public class CreateCustomerCommand:IRequest<CreatedCustomerResponse>
{
    public CreateCustomerRequest customerRequest { get; set; }

    class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreatedCustomerResponse>
    {

        public readonly IMapper _mapper;
        public readonly ICustomerService _repository;

        public CreateCustomerCommandHandler(IMapper mapper,ICustomerService repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<CreatedCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            Customer customer = _mapper.Map<Customer>(request.customerRequest);
            HashingHelper.CreatePasswordHash
                (
                request.customerRequest.Password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
                );

            customer.PasswordHash = passwordHash;
            customer.PasswordSalt = passwordSalt;

            await _repository.AddAsync(customer);

            CreatedCustomerResponse response = _mapper.Map<CreatedCustomerResponse>(customer);

            return response;
        }
    }
}
