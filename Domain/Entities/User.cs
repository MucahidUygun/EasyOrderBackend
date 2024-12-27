using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class User : BaseUser
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string IdentityNumber { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set;}
}
