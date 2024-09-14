using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Azure.Storage.Blobs;
using Main.Consts;
using Main.Models;
using Main.Services;
using Main.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class SellCardController : ControllerBase<SellCardController> {
    private readonly IMapper _mapper;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerImageName;
    public SellCardController(Context context, ILogger<SellCardController> logger, BlobServiceClient blobServiceClient, IMapper mapper) : base(context, logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerImageName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<SellCard, SellCardDto>()
            .ForMember(p => p.CategoryName, p => p.MapFrom(p => p.SellCardCategory.Name ?? ""));
        });
        _mapper = config.CreateMapper();
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, [FromQuery] string sellCardCategoryId) {
        var sellCardService = new SellCardService(_context);
        var result = sellCardService.List(paginationRequest, sellCardCategoryId);
        var listDto = _mapper.Map<List<SellCardDto>>(result.Items);
        var resultDto = new PaginationResult<SellCardDto>(listDto, result.TotalCount, result.PageNumber, result.MaxPageNumber, result.PageSize);
        AddSasToken(resultDto.Items);
        return new OkObjectResult(resultDto);
    }

    // [HttpGet]
    // [Route("list/account")]
    // public IActionResult ListByAccount([FromQuery] PaginationRequest paginationRequest, [FromQuery] string accountId) {
    //     var cardService = new SellCardService(_context);
    //     var result = cardService.List(paginationRequest, accountId);
    //     var listDto = _mapper.Map<List<SellCardDto>>(result.Items).Select(p => { p.CardCategory = null; return p; }).ToList();
    //     var resultDto = new PaginationResult<SellCardDto>(listDto, result.TotalCount, result.PageNumber, result.MaxPageNumber, result.PageSize);
    //     AddSasToken(resultDto.Items);
    //     return new OkObjectResult(resultDto);
    // }

    private void AddSasToken(SellCardDto card) {
        var fileService = new FileService(_blobServiceClient);
        if(!string.IsNullOrEmpty(card.ClueImg)) AddSasToken(fileService, card);
    }

    private void AddSasToken(List<SellCardDto> cards) {
        var fileService = new FileService(_blobServiceClient);
        foreach(var card in cards) {
            if(!string.IsNullOrEmpty(card.ClueImg)) AddSasToken(fileService, card);
        }
    }

    private void AddSasToken(FileService fileService, SellCardDto card) {
        card.ClueImgUrl = fileService.GetSasToken(_containerImageName, card.ClueImg ?? "");
    }
}

public class SellCardDto : Card {
    public string? ClueImgUrl { get; set; }
    public string? CategoryName { get; set; }
}