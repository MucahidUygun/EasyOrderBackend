using AutoMapper;
using Domain.Entities;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.OperationClaims;

public class OperationClaimManager : IOperationClaimService
{
    public readonly IOperationClaimRepository _operationClaimRepository;
    public readonly IMapper _mapper;


    public OperationClaimManager(IOperationClaimRepository operationClaimRepository,IMapper mapper)
    {
        _mapper = mapper;
        _operationClaimRepository = operationClaimRepository;
    }

    public async Task<OperationClaim> AddAsync(OperationClaim operationClaim, CancellationToken cancellationToken = default)
    {
        OperationClaim? lastClaim = await _operationClaimRepository.GetLastAsync(x=>x.Id);
        await _operationClaimRepository.AddAsync(operationClaim);

        return operationClaim;
    }
}
