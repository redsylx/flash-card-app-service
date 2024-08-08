using Main.Extensions;
using Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.Consts;

namespace Main.Controllers;

public class AuthController : ControllerBase<AuthController> {
    private readonly HttpContext? _httpContextAccessor;

    public AuthController(ILogger<AuthController> logger, Context context, IHttpContextAccessor haccess) : base(context, logger)
    {
        _httpContextAccessor = haccess?.HttpContext;
    }

    [HttpPut]
    public IActionResult Put([FromQuery] string username) {
        var email = _httpContextAccessor.GetClaim(ClaimTypeConst.Email);
        var accountService = new AccountService(_context);
        accountService.CreateAccount(email, username);
        return new OkResult();
    }
}