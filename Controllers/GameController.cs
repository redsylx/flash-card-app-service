using System.Linq;
using Main.Exceptions;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.DTO.GameDto;
using Main.Consts;

namespace Main.Controllers;

public class GameController : ControllerBase<GameController> {
    public GameController(Context context, ILogger<GameController> logger) : base(context, logger)
    {
    }

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

    [HttpGet]
    public IActionResult Get([FromQuery] string accountId, string gameId) {
        var gameService = new GameService(_context);
        var game = gameService.Get(accountId, gameId);
        return new OkObjectResult(game);
    }

    [HttpPut]
    [Route("finish")]
    public IActionResult Put([FromQuery] string gameId) {
        var gameService = new GameService(_context);
        var cardService = new CardService(_context);
        var gameDetailService = new GameDetailService(_context);
        var cardVersionService = new CardVersionService(_context);

        var game = gameService.Finish(gameId);
        cardService.Finish(gameId);

        var nGameFinish = _context.Game.Count(p => p.Account != null && game.Account != null && p.Account.Id == game.Account.Id && p.Status == GameConst.FINISH);
        if(nGameFinish > 10) {
            var deletedCardVersionIds = gameDetailService.Delete(game.Id, game.Account?.Id ?? "");
            cardVersionService.Delete(deletedCardVersionIds);
            _context.SaveChanges();
        }
        
        return new OkResult();
    }

    [HttpGet]
    [Route("list")]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, string accountId) {
        if(string.IsNullOrEmpty(accountId)) throw new BadRequestException("accountId is missing from query");
        var gameService = new GameService(_context);
        return new OkObjectResult(gameService.List(paginationRequest, accountId));
    }
}