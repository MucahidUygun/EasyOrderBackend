using Application.Features.Customers.Constants;
using Application.Services.Customers;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Rules;

public class CustomerBusinessRules : BaseBusinessRules
{
    private readonly ICustomerService _service;

    public CustomerBusinessRules(ICustomerService service)
    {
        _service = service;
    }

    public async Task CustomerIdShouldExistWhenSelected(Guid id,CancellationToken cancellationToken)
    {
        bool customer = await _service.AnyAsync(p=>p.Id==id,cancellationToken:cancellationToken);
        if (!customer)
            throw new BusinessException(CustomersBusinessMessages.CustomerNotExists);
    }
}
