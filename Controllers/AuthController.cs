using Main.Extensions;
using Main.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.Consts;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Main.Controllers;

public class AuthController : ControllerBase<AuthController> {
    private readonly HttpContext? _httpContextAccessor;

    public AuthController(ILogger<AuthController> logger, Context context, IHttpContextAccessor haccess) : base(context, logger)
    {
        _httpContextAccessor = haccess?.HttpContext;
    }

    [HttpGet]
    public IActionResult Get() {
        var email = _httpContextAccessor.GetEmail();
        var newAccount = new AccountService(_context).CheckAccount(email);
        new CartService(_context).Create(newAccount.Id);
        if(string.IsNullOrEmpty(newAccount.Username)) {
            try {
                new CardCategoryService(_context).Create(newAccount.Id, DefaultNameConst.CARD_CATEGORY);
            } catch (Exception e) {
            }
        }
        return new OkObjectResult(new { username = newAccount.Username, id = newAccount.Id, point = newAccount.Point });
    }

    [HttpPut]
    public IActionResult Put([FromQuery] string username) {
        var email = _httpContextAccessor.GetEmail();
        var accountService = new AccountService(_context);
        accountService.UpdateAccountUsername(email, username);
        return new OkResult();
    }
}