using System.Collections.Generic;
using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class CardCategoryController : ControllerBase<CardCategoryController> {
    public CardCategoryController(Context context, ILogger<CardCategoryController> logger) : base(context, logger)
    {
    }

    [HttpGet]
    public ActionResult<List<CardCategory>> GetList([FromQuery] string accountId) {
        var cardCategoryService = new CardCategoryService(_context);
        return new OkObjectResult(cardCategoryService.GetCardCategories(accountId));
    }

    [HttpPost]
    public ActionResult<List<CardCategory>> Create([FromQuery] string accountId, string name) {
        var cardCategoryService = new CardCategoryService(_context);
        return new OkObjectResult(cardCategoryService.CreateCardCategory(accountId, name));
    }
}