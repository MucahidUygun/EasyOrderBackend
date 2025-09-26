using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.IndividualCustomers.Dtos.Requests;

public class GetByIdIndividualCustomerQueryRequest
{
    public Guid Id { get; set; }
}
