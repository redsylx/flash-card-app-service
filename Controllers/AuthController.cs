using System;
using Main.Exceptions;
using Main.Extensions;
using Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.Consts;

namespace Main.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {
    private readonly ILogger<AuthController> _logger;
    private readonly HttpContext? _httpContextAccessor;

    public AuthController(ILogger<AuthController> logger, Context context, IHttpContextAccessor haccess) : base(context)
    {
        _logger = logger;
        _httpContextAccessor = haccess?.HttpContext;
    }

    [HttpPut]
    [Authorize]
    public IActionResult Put([FromQuery] string username) {
        try {
            var email = _httpContextAccessor.GetClaim(ClaimTypeConst.Email);
            var accountService = new AccountService(_context);
            accountService.CreateAccount(email, username);
            return new OkResult();
        } catch (BadRequestException e) {   
            return new BadRequestObjectResult(e.Message);
        } catch (Exception e) {
            _logger.LogError(e.ToString());
            return new InternalServerErrorException(e.Message);
        }
    }
}