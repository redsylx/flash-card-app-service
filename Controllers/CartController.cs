using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.Consts;
using AutoMapper;
using Azure.Storage.Blobs;
using System;
using Microsoft.AspNetCore.Authorization;
using Main.Utils;
using System.Collections.Generic;
using System.Linq;
using Main.Models;

namespace Main.Controllers;

public class CartController : ControllerBase<CartController> {
    public CartController(Context context, ILogger<CartController> logger) : base(context, logger)
    {
    }

    [HttpGet]
    public IActionResult Get([FromQuery] string accountId) {
        var cartService = new CartService(_context);
        return new OkObjectResult(cartService.GetByAccountId(accountId));
    }
    
}