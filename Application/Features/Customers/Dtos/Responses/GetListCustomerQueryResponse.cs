using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Dtos.Responses;

public class GetListCustomerQueryResponse
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int Debit { get; set; }
}
