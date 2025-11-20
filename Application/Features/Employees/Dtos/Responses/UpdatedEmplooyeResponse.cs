using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Dtos.Responses;

public class UpdatedEmplooyeResponse
{
    public string PhoneNumber { get; set; }
    public string Adress { get; set; }
    public string Email { get; set; }
    public int Sallary { get; set; }
    public string Position { get; set; }
}
