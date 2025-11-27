using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class EmailAuthenticator : BaseEmailAuthenticators<Guid>
{
    public bool? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpiry { get; set; }
    public virtual User User { get; set; } = default!;
}
