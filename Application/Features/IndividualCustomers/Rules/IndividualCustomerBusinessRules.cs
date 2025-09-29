using Application.Features.IndividualCustomers.Constants;
using Application.Services.IndividualCustomers;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Expeptions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.IndividualCustomers.Rules;

public class IndividualCustomerBusinessRules : BaseBusinessRules
{
    private readonly IIndividualCustomerService _service;

    public IndividualCustomerBusinessRules(IIndividualCustomerService service)
    {
        _service = service;
    }

    public async Task CustomerIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken = default)
    {
        bool customer = await _service.AnyAsync(p => p.Id == id, cancellationToken: cancellationToken);
        if (!customer)
            throw new BusinessException(IndividualCustomersBusinessMessages.IndividualCustomerNotExists);
    }
}
