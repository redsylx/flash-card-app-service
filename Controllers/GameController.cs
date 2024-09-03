using System.Linq;
using Main.Exceptions;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.DTO.GameDto;
using Main.Consts;
using Main.Models;
using AutoMapper;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;

namespace Main.Controllers;

public class GameController : ControllerBase<GameController> {
    private readonly IMapper _mapper;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerImageName;
    public GameController(Context context, ILogger<GameController> logger, BlobServiceClient blobServiceClient) : base(context, logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerImageName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<Game, GameDto>()
            .ForMember(p => p.CreatedTime, p => p.MapFrom(p => DateTime.SpecifyKind(p.CreatedTime, DateTimeKind.Utc)))
            .ForMember(p => p.Categories, p => p.Ignore())
            .ForMember(p => p.Details, p => p.Ignore());
        });
        _mapper = config.CreateMapper();
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateGameDTO dto) {
        if(dto is null) throw new BadRequestException("CreateGameDTO is required");
        var cardService = new CardService(_context);
        var generatedCards = cardService.Generate(dto.NCard, dto.CategoryIds);
        var gameService = new GameService(_context);
        var palyingGames = gameService.GetPlayingGames(dto.AccountId);
        var gameDetailService = new GameDetailService(_context);
        var gameDetailCategoryService = new GameDetailCategoryService(_context);
        gameDetailService.DeleteAll(palyingGames.Select(p => p.Id).ToList());
        gameDetailCategoryService.DeleteAll(palyingGames.Select(p => p.Id).ToList());
        gameService.DeletePlayingGames(dto.AccountId);
        var newGame = gameService.Create(dto.AccountId, dto.NCard, dto.HideDurationInSecond);
        gameDetailService.Create(generatedCards.Select(p => p.Id).ToList(), newGame.Id);
        gameDetailCategoryService.Create(dto.CategoryIds, newGame.Id);
        newGame.Account = null;
        return new OkObjectResult(newGame);
    }

    [HttpGet]
    public IActionResult Get([FromQuery] string accountId, string gameId) {
        var gameService = new GameService(_context);
        var game = gameService.Get(accountId, gameId);
        return new OkObjectResult(game);
    }
    
    [HttpGet]
    [Route("resume")]
    public IActionResult GetResume([FromQuery] string accountId) {
        var gameService = new GameService(_context);
        var game = gameService.GetResume(accountId);
        if(game == null) {
            game = new Game();
            game.Id = "";
        }
        return new OkObjectResult(game);
    }

    [HttpPut]
    [Route("finish")]
    public IActionResult Put([FromQuery] string gameId) {
        var gameService = new GameService(_context);
        var cardService = new CardService(_context);
        var gameDetailService = new GameDetailService(_context);
        var gameDetailCategoryService = new GameDetailCategoryService(_context);
        var pointActivityService = new PointActivityService(_context);
        var accountService = new AccountService(_context);

        var game = gameService.Finish(gameId);
        cardService.Finish(gameId);

        var nGameFinish = _context.Game.Count(p => p.Account != null && game.Account != null && p.Account.Id == game.Account.Id && p.Status == GameConst.FINISH);

        // if(nGameFinish > 5) {
        //     var deletedGameIds = gameService.GetGamesToBeDeleted(game.Account.Id, 5).Select(p => p.Id).ToList();
        //     gameDetailCategoryService.DeleteAll(deletedGameIds);
        //     gameDetailService.DeleteAll(deletedGameIds);
        //     gameService.DeleteAll(deletedGameIds);
        // }
        
        var pointActivity = pointActivityService.FinishGame(game.Id, game.Account.Id);
        accountService.UpdatePoint(game.Account.Id, pointActivity.Id);

        return new OkObjectResult(game);
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, string accountId) {
        if (string.IsNullOrEmpty(accountId)) 
            throw new BadRequestException("accountId is missing from query");

        var gameService = new GameService(_context);

        if (!paginationRequest.IsPaged) {
            var games = gameService.List(accountId).OrderByDescending(p => p.CreatedTime);
            var gameDtos = _mapper.Map<List<GameDto>>(games);

            foreach (var dto in gameDtos) {
                var originGame = games.FirstOrDefault(p => p.Id == dto.Id);
                if (originGame != null) {
                    dto.ListCategory = originGame.Categories.OrderBy(p => p.Name).Select(c => c.Name).ToList();
                }
            }

            return new OkObjectResult(gameDtos);
        }

        return new OkObjectResult(gameService.List(paginationRequest, accountId));
    }

}

public class GameDto : Game { 
    public List<string> ListCategory { get; set; } = new();
}