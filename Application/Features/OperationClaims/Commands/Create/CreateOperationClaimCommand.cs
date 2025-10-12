using Application.Features.OperationClaims.Dtos.Requests;
using Application.Features.OperationClaims.Dtos.Responses;
using Application.Services.OperationClaims;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimCommand : IRequest<CreatedOperationClaimCommandResponse>
{
    public CreateOperationClaimCommandRequest _operationClaimCommandRequest { get; set; }

    public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CreatedOperationClaimCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IOperationClaimService _operationClaimService;

        public CreateOperationClaimCommandHandler(IMapper mapper, IOperationClaimService operationClaimService)
        {
            _mapper = mapper;
            _operationClaimService = operationClaimService;
        }

        public async Task<CreatedOperationClaimCommandResponse> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
        {

            OperationClaim operationClaim = _mapper.Map<OperationClaim>(request._operationClaimCommandRequest);

            OperationClaim responseClaim = await _operationClaimService.AddAsync(operationClaim);

            CreatedOperationClaimCommandResponse response = _mapper.Map<CreatedOperationClaimCommandResponse>(responseClaim);
            return response;
        }
    }
}
