using Application.Features.CorporateCustomers.Commands.Update;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Rules.Validators;

public class UpdateCorporateCustomerCommandValidator : AbstractValidator<UpdateCorporateCustomerCommand>
{
    public UpdateCorporateCustomerCommandValidator()
    {
        RuleFor(p=>p.updateCorporateCustomerRequest.Email).EmailAddress().MinimumLength(2);
    }
}
