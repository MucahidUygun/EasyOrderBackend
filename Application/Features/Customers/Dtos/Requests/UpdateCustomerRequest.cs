using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Dtos.Requests;

public class UpdateCustomerRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string IdentityNumber { get; set; }
    public string ChargeName { get; set; }
    public int Debit { get; set; }
}
