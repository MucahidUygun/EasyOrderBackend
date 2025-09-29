using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CorporateCustomers.Dtos.Responses;

public class UpdatedCorporateCustomerResponse
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string TaxNumber { get; set; }
    public string TaxOffice { get; set; }
    public string Email { get; set; }
    public string Adress { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public int AccountBalance { get; set; }
}
