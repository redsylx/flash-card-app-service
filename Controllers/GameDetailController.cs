using Main.Exceptions;
using Main.Models;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class GameDetailController : ControllerBase<GameDetailController> {
    public GameDetailController(Context context, ILogger<GameDetailController> logger) : base(context, logger)
    {
    }

    [HttpPut]
    [AllowAnonymous]
    public IActionResult Put([FromBody] GameDetail dto) {
        var gameDetailService = new GameDetailService(_context);
        gameDetailService.Answer(dto);
        return new OkResult();
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, string gameId) {
        if(string.IsNullOrEmpty(gameId)) throw new BadRequestException("gameId is missing from query");
        var gameDetailService = new GameDetailService(_context);
        return new OkObjectResult(gameDetailService.List(paginationRequest, gameId));
    }
}