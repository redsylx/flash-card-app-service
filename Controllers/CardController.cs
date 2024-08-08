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
        return new OkObjectResult(cardService.CreateCard(dto.CardCategory?.Id ?? "", dto.ClueTxt, dto.DescriptionTxt));
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest) {
        var cardService = new CardService(_context);
        return new OkObjectResult(cardService.List(paginationRequest));
    }
}