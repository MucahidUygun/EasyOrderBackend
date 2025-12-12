using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Responses;

public class ForgotPasswordResponse
{
    public string? Message { get; set; }

    public ForgotPasswordResponse()
    {
        
    }
    public ForgotPasswordResponse(string? message)
    {
        Message = message;
    }
}
