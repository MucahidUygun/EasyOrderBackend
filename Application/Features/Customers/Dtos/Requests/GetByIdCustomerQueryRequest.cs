using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Dtos.Requests;

public class GetByIdCustomerQueryRequest
{
    public Guid Id { get; set; }
}
