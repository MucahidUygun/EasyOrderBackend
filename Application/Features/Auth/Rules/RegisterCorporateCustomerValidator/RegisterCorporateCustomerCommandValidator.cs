using Application.Features.Auth.Commands.Registers.RegisterCustomers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Rules.RegisterCorporateCustomerValidator;

public class RegisterCorporateCustomerCommandValidator : AbstractValidator<RegisterCorporateCustomerCommand>
{
    public RegisterCorporateCustomerCommandValidator()
    {
        RuleFor(x => x.RegisterCorporateCustomerRequests)
    .NotNull()
    .WithMessage("Request null")
    .Must(x =>
    {
        Console.WriteLine("ChildRules çalıştı");
        return true;
    });

        RuleFor(p => p.RegisterCorporateCustomerRequests).NotNull().ChildRules(
            request =>
            {
                request.RuleFor(p => p.CompanyName).NotEmpty().MinimumLength(2).WithMessage("Düzgün isim girin");
                request.RuleFor(p => p.Email).NotEmpty().MinimumLength(2);
            }
        );
    }
}
