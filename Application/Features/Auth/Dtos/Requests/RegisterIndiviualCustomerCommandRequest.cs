using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Requests;

public class RegisterIndiviualCustomerCommandRequest
{
    public string? PhoneNumber { get; set; }
    public string? Adress { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string FirstName { get; set; }
    public string lastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string IdentityNumber { get; set; }
}
