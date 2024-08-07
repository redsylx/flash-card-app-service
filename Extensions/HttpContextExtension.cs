using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Main.Extensions;

public static class HttpContextExtension {
    public static string GetClaim(this HttpContext? context, string claimType) 
    {
        return context?.User?.Claims?.FirstOrDefault(p => p.Type == claimType)?.Value ?? "";
    }
}