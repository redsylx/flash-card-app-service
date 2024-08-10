using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class CardVersionController : ControllerBase<CardVersionController> {
    public CardVersionController(Context context, ILogger<CardVersionController> logger) : base(context, logger)
    {
    }

    [HttpGet]
    [Route("list/{cardId}")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, string cardId) {
        var cardVersionService = new CardVersionService(_context);
        return new OkObjectResult(cardVersionService.List(paginationRequest, cardId));
    }
}