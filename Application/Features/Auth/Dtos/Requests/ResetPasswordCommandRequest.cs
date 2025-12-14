using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Requests;

public class ChangePasswordCommandRequest
{
    public string? Password { get; set; }
    public string? RePassword { get; set; }
}
