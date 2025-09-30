using Application.Features.CorporateCustomers.Constants;
using Application.Services.CorporateCustomers;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Expeptions.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Rules;

public class CorporateCustomerBusinessRules : BaseBusinessRules
{
    private readonly ICorporateCustomerService _corporateCustomerService;
    public CorporateCustomerBusinessRules(ICorporateCustomerService corporateCustomerService)
    {
        _corporateCustomerService = corporateCustomerService;
    }

    public async Task CorporateCustomerIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken = default)
    {
        bool isHave = await _corporateCustomerService.AnyAsync(p => p.Id == id, cancellationToken: cancellationToken);
        if (!isHave)
            throw new BusinessException(CorporateCustomerBusinessMessages.CorporateCustomerNotExists);
    }
}
