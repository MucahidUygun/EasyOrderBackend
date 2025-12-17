using Application.Features.CorporateCustomers.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Rules.Validators;

public class CorporateCustomerGetByIdValidator : AbstractValidator<GetByIdCorporateCustomerQuery>
{
    public CorporateCustomerGetByIdValidator()
    {
        RuleFor(p => p.getByIdCorporateCustomerRequest.Id).NotNull().NotEmpty();
    }
}
