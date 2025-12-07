using Core.Entities;
using Core.Security.JWT;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Dtos.Responses;
public class RegisteredResponse
{
    public string? Message { get; set; }
    
}