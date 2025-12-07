using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Requests;

public class VerifyEmailCommandRequest
{
    public Guid Id { get; set; }
    public string ActivationKey { get; set; }
}
