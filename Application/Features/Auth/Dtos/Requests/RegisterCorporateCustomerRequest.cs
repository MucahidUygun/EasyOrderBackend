using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Requests;

public class RegisterCorporateCustomerRequest
{
    public string? PhoneNumber { get; set; }
    public string? Adress { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxNumber { get; set; }
    public string? TaxOffice { get; set; }
}
