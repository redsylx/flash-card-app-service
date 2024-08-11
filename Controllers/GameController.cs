using System.Linq;
using Main.Exceptions;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.DTO.GameDto;

namespace Main.Controllers;

public class GameController : ControllerBase<GameController> {
    public GameController(Context context, ILogger<GameController> logger) : base(context, logger)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromBody] CreateGameDTO dto) {
        if(dto is null) throw new BadRequestException("CreateGameDTO is required");
        var cardService = new CardService(_context);
        var generatedCards = cardService.Generate(dto.NCard, dto.CategoryIds);
        var gameService = new GameService(_context);
        var newGame = gameService.Create(dto.AccountId, dto.NCard, dto.HideDurationInSecond);
        var gameDetailService = new GameDetailService(_context);
        gameDetailService.Create(generatedCards.Select(p => p.CurrentVersionId).ToList(), newGame.Id);
        newGame.Account = null;
        return new OkObjectResult(newGame);
    }

    [HttpPut]
    [AllowAnonymous]
    [Route("finish")]
    public IActionResult Put([FromQuery] string gameId) {
        var gameService = new GameService(_context);
        gameService.Finish(gameId);
        var cardService = new CardService(_context);
        cardService.Finish(gameId);
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