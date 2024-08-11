using Main.Exceptions;
using Main.Models;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class GameController : ControllerBase<GameController> {
    public GameController(Context context, ILogger<GameController> logger) : base(context, logger)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromBody] Game dto) {
        var gameService = new GameService(_context);
        var newCard = gameService.Create(dto.Account?.Id ?? "", dto.NCard, dto.HideDurationInSecond);
        newCard.Account = null;
        return new OkObjectResult(newCard);
    }

    [HttpPut]
    [AllowAnonymous]
    [Route("finish")]
    public IActionResult Put([FromQuery] string gameId) {
        var gameService = new GameService(_context);
        var game = gameService.Finish(gameId);
        return new OkResult();
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, string accountId) {
        if(string.IsNullOrEmpty(accountId)) throw new BadRequestException("accountId is missing from query");
        var gameService = new GameService(_context);
        return new OkObjectResult(gameService.List(paginationRequest, accountId));
    }
}