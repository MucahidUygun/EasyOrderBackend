using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Dtos.Responses;

public class GetByIdCustomerQueryResponse
{
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int AccountBalance { get; set; }
}
