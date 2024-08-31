using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Azure.Storage.Blobs;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class GameDetailController : ControllerBase<GameDetailController> {
    private readonly IMapper _mapper;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerImageName;
    public GameDetailController(Context context, ILogger<GameDetailController> logger, BlobServiceClient blobServiceClient, IMapper mapper) : base(context, logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerImageName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<Card, CardDto>()
                .ForMember(p => p.CardCategory, p => p.Ignore());
            cfg.CreateMap<GameDetail, GameDetailDto>()
                .ForMember(p => p.CategoryName, c => c.MapFrom(p => p.CategoryName));
        });
        _mapper = config.CreateMapper();
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
        if(!paginationRequest.IsPaged) {
            var data = gameDetailService.List(gameId);
            var result = _mapper.Map<List<GameDetailDto>>(data);
            AddSasToken(result);
            return new OkObjectResult(result);
        };
        return new OkObjectResult(gameDetailService.List(paginationRequest, gameId));
    }

    private void AddSasToken(GameDetailDto gameDetail) {
        var fileService = new FileService(_blobServiceClient);
        if(!string.IsNullOrEmpty(gameDetail.ClueImg)) AddSasToken(fileService, gameDetail);
    }

    private void AddSasToken(List<GameDetailDto> gameDetails) {
        if(gameDetails == null) return;
        var fileService = new FileService(_blobServiceClient);
        foreach(var card in gameDetails) {
            if(card == null) continue; 
            if(!string.IsNullOrEmpty(card.ClueImg)) AddSasToken(fileService, card);
        }
    }

    private void AddSasToken(FileService fileService, GameDetailDto gameDetail) {
        gameDetail.ClueImgUrl = fileService.GetSasToken(_containerImageName, gameDetail.ClueImg ?? "");
    }

    public class GameDetailDto : GameDetail {
        public string ClueImgUrl { get; set; } = "";
    }
}