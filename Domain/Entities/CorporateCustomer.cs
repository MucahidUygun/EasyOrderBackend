using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class CorporateCustomer : Customer
{
    //public string CurrentAccountCode { get; set; }
    public string CompanyName { get; set; }
    public string TaxNumber { get; set; }
    public string TaxOffice { get; set; }
}
