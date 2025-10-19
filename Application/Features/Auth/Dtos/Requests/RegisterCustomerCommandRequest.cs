using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Requests;

public class RegisterCustomerCommandRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Adress { get; set; }
    public string PhoneNumber { get; set; }
    public int AccountBalance { get; set; }
}
