using Core.Constants;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static ICollection<string>? GetClaims(this ClaimsPrincipal claimsPrincipal, string claimType)
    {
        //Bütün cliamler arasında cliamType' a göre dolaşılıyor.
        return claimsPrincipal?.FindAll(claimType)?.Select(x => x.Value).ToImmutableArray();
    }

    public static ICollection<string>? ClaimRoles(this ClaimsPrincipal claimsPrincipal)
    {
        //Hangi cliam türüne göre filtreleniyor
        return claimsPrincipal?.GetClaims(CoreMessages.CliamRole);
    }

    public static string? GetIdClaim(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal?.GetEmail();
    }
}