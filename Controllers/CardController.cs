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

public class CardController : ControllerBase<CardController> {
    private readonly IMapper _mapper;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerImageName;
    public CardController(Context context, ILogger<CardController> logger, BlobServiceClient blobServiceClient, IMapper mapper) : base(context, logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerImageName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<Card, CardDto>();
        });
        _mapper = config.CreateMapper();
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromBody] Card dto) {
        var cardService = new CardService(_context);
        var newCard = cardService.CreateCard(dto.CardCategory?.Id ?? "", dto.ClueTxt, dto.DescriptionTxt, dto.ClueImg, dto.DescriptionImg);
        var cardCategoryService = new CardCategoryService(_context);
        cardCategoryService.CountNCard(dto.CardCategory?.Id ?? "");
        var cardVersionService = new CardVersionService(_context);
        cardVersionService.CreateOrUpdate(newCard.CurrentVersionId, newCard.Id, newCard.ClueTxt, newCard.DescriptionTxt, newCard.ClueImg, newCard.DescriptionImg);
        var returnDto = _mapper.Map<CardDto>(newCard);
        AddSasToken(returnDto);
        return new OkObjectResult(returnDto);
    }

    [HttpPut]
    [AllowAnonymous]
    public IActionResult Put([FromBody] Card dto) {
        var cardService = new CardService(_context);
        var updatedCard = cardService.Update(dto.Id, dto.ClueTxt, dto.DescriptionTxt, dto.ClueImg, dto.DescriptionImg);
        var cardVersionService = new CardVersionService(_context);
        var cardVersion = cardVersionService.CreateOrUpdate(updatedCard.CurrentVersionId, updatedCard.Id, updatedCard.ClueTxt, updatedCard.DescriptionTxt, updatedCard.ClueImg, updatedCard.DescriptionImg);
        updatedCard.CurrentVersionId = cardVersion.Id;
        _context.Card.Update(updatedCard);
        _context.SaveChanges();
        var returnDto = _mapper.Map<CardDto>(updatedCard);
        AddSasToken(returnDto);
        return new OkObjectResult(returnDto);
    }

    [HttpGet]
    [Route("list")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, [FromQuery] string cardCategoryId) {
        var cardService = new CardService(_context);
        var result = cardService.List(paginationRequest, cardCategoryId);
        var listDto = _mapper.Map<List<CardDto>>(result.Items);
        var resultDto = new PaginationResult<CardDto>(listDto, result.TotalCount, result.PageNumber, result.MaxPageNumber, result.PageSize);
        AddSasToken(resultDto.Items);
        return new OkObjectResult(resultDto);
    }

    private void AddSasToken(CardDto card) {
        var fileService = new FileService(_blobServiceClient);
        if(!string.IsNullOrEmpty(card.ClueImg)) AddSasToken(fileService, card);
    }

    private void AddSasToken(List<CardDto> cards) {
        var fileService = new FileService(_blobServiceClient);
        foreach(var card in cards) {
            if(!string.IsNullOrEmpty(card.ClueImg)) AddSasToken(fileService, card);
        }
    }

    private void AddSasToken(FileService fileService, CardDto card) {
        card.ClueImgUrl = fileService.GetSasToken(_containerImageName, card.ClueImg ?? "");
    }

    public class CardDto : Card {
        public string? ClueImgUrl { get; set; }
    }
}