using System;
using System.Collections.Generic;
using System.Linq;
using Main.Models;
using Main.Services;
using Microsoft.AspNetCore.Authorization;
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
        return new OkObjectResult(cardCategoryService.Create(accountId, name));
    }

    [HttpDelete]
    public ActionResult Delete([FromQuery] string accountId, string categoryId) {
        var cardCategoryService = new CardCategoryService(_context);
        var _ = cardCategoryService.Get(categoryId) ?? throw new Exception("Invalid categoryId");
        var cardService = new CardService(_context);
        cardService.DeleteByCategory(categoryId);
        cardCategoryService.Delete(accountId, categoryId);
        return new OkResult();
    }

    [HttpPut]
    public ActionResult Update([FromQuery] string accountId, string categoryId, string newCategoryName) {
        var cardCategoryService = new CardCategoryService(_context);
        cardCategoryService.Update(accountId, categoryId, newCategoryName);
        return new OkResult();
    }

    [HttpPost]
    [Route("generate")]
    [AllowAnonymous]
    public ActionResult Generate([FromBody] GenerateDto dto, [FromQuery] string categoryId) {
        var category = _context.CardCategory.FirstOrDefault(p => p.Id == categoryId);
        var cardService = new CardService(_context);
        foreach(var p in dto.Cards) {
            cardService.CreateCard(categoryId, p.ClueTxt, p.DescriptionTxt, null, null);
        }
        return new OkResult();
    }

    [HttpGet]
    [Route("convert")]
    [AllowAnonymous]
    public IActionResult Convert([FromQuery] string accountId, string sellCardCategoryId, string newCategoryName) {
        var cardCategoryService = new CardCategoryService(_context);
        var cardService = new CardService(_context);
        var newCardCategory = cardCategoryService.Convert(accountId, sellCardCategoryId, newCategoryName);
        cardService.Convert(newCardCategory.Id, sellCardCategoryId);
        return new OkResult();
    }
}


public class GenerateDto {
    public Card[] Cards { get; set; }
}