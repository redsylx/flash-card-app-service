using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Main.Extensions;

public static class HttpContextExtension {
    public static string GetClaim(this HttpContext? context, string claimType) 
    {
        return context?.User?.Claims?.FirstOrDefault(p => p.Type == claimType)?.Value ?? "";
    }

    public static string GetEmail(this HttpContext? context) 
    {
        return context?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
    }
}