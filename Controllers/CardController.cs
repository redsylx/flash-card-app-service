using Main.Models;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class CardController : ControllerBase<CardController> {
    public CardController(Context context, ILogger<CardController> logger) : base(context, logger)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromBody] Card dto) {
        var cardService = new CardService(_context);
        var newCard = cardService.CreateCard(dto.CardCategory?.Id ?? "", dto.ClueTxt, dto.DescriptionTxt, dto.ClueImg, dto.DescriptionImg);
        var cardCategoryService = new CardCategoryService(_context);
        cardCategoryService.CountNCard(dto.CardCategory?.Id ?? "");
        var cardVersionService = new CardVersionService(_context);
        cardVersionService.Create(newCard.CurrentVersionId, newCard.Id, newCard.ClueTxt, newCard.DescriptionTxt, newCard.ClueImg, newCard.DescriptionImg);
        return new OkObjectResult(newCard);
    }

    [HttpPut]
    [AllowAnonymous]
    public IActionResult Put([FromBody] Card dto) {
        var cardService = new CardService(_context);
        var updatedCard = cardService.Update(dto.Id, dto.ClueTxt, dto.DescriptionTxt, dto.ClueImg, dto.DescriptionImg);
        var cardVersionService = new CardVersionService(_context);
        cardVersionService.Create(updatedCard.CurrentVersionId, updatedCard.Id, updatedCard.ClueTxt, updatedCard.DescriptionTxt, updatedCard.ClueImg, updatedCard.DescriptionImg);
        return new OkObjectResult(updatedCard);
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, [FromQuery] string cardCategoryId) {
        var cardService = new CardService(_context);
        return new OkObjectResult(cardService.List(paginationRequest, cardCategoryId));
    }
}