using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IJwtService
{
    public string GetIdFromOldAccesToken(string accessToken);
    public string GetEmailFromOldAccesToken(string accessToken);
    public List<BaseClaim> GetClaimsByKey(string accessToken, string claimName);
}
