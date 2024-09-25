using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.Consts;
using AutoMapper;
using Azure.Storage.Blobs;
using System;
using Microsoft.AspNetCore.Authorization;
using Main.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Main.Controllers;

public class SellCardCategoryController : ControllerBase<SellCardCategoryController> {
    private readonly IMapper _mapper;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerImageName;
    public SellCardCategoryController(Context context, ILogger<SellCardCategoryController> logger, BlobServiceClient blobServiceClient) : base(context, logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerImageName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<SellCardCategory, SellCardCategoryDto>();
        });
        _mapper = config.CreateMapper();
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromBody] SellCardCategory dto, [FromQuery] string accountId, [FromQuery] string cardCategoryId) {
        var sellCardCategoryService = new SellCardCategoryService(_context);
        var sellCardService = new SellCardService(_context);

        var sellCardCategory = sellCardCategoryService.Create(accountId, cardCategoryId, dto);
        sellCardService.Create(sellCardCategory.Id, cardCategoryId);

        return new OkObjectResult(sellCardCategory);
    }

    [HttpGet]
    [Route("list/account")]
    public IActionResult ListByAccount([FromQuery] PaginationRequest paginationRequest, [FromQuery] string accountId) {
        var sellCardCategoryService = new SellCardCategoryService(_context);
        var result = sellCardCategoryService.ListByAccount(paginationRequest, accountId);
        var listDto = _mapper.Map<List<SellCardCategoryDto>>(result.Items).Select(p => { p.Account = null; return p; }).ToList();
        var resultDto = new PaginationResult<SellCardCategoryDto>(listDto, result.TotalCount, result.PageNumber, result.MaxPageNumber, result.PageSize);
        AddSasToken(resultDto.Items);
        return new OkObjectResult(resultDto);
    }

    [HttpGet]
    [Route("list/account/exclude")]
    [AllowAnonymous]
    public IActionResult List([FromQuery] PaginationRequest paginationRequest, [FromQuery] string accountId) {
        var sellCardCategoryService = new SellCardCategoryService(_context);
        var result = sellCardCategoryService.ListExcludeAccount(paginationRequest, accountId);
        var listDto = _mapper.Map<List<SellCardCategoryDto>>(result.Items).Select(p => {
            p.Account.Id = "";
            p.Account.Email = "";
            p.Account.Id = "";
            p.Account.CreatedTime = DateTime.MinValue;
            p.Account.LastUpdatedTime = DateTime.MinValue;
            p.Account.Point = 0;
            return p;
        }).ToList();
        var resultDto = new PaginationResult<SellCardCategoryDto>(listDto, result.TotalCount, result.PageNumber, result.MaxPageNumber, result.PageSize);
        AddSasToken(resultDto.Items);
        return new OkObjectResult(resultDto);
    }

    [HttpGet]
    [Route("list/buyer")]
    public IActionResult ListByBuyer([FromQuery] PaginationRequest paginationRequest, [FromQuery] string transactionId) {
        var transactionDetailService = new TransactionDetailService(_context);
        var result = transactionDetailService.ListByBuyerTransactionId(paginationRequest, transactionId);
        var listDto = _mapper.Map<List<SellCardCategoryDto>>(result.Items.Select(p => {
            p.SellCardCategory.Account.Id = "";
            p.SellCardCategory.Account.Email = "";
            p.SellCardCategory.Account.Id = "";
            p.SellCardCategory.Account.CreatedTime = DateTime.MinValue;
            p.SellCardCategory.Account.LastUpdatedTime = DateTime.MinValue;
            p.SellCardCategory.Account.Point = 0;
            return p.SellCardCategory;
        })).ToList();
        var resultDto = new PaginationResult<SellCardCategoryDto>(listDto, result.TotalCount, result.PageNumber, result.MaxPageNumber, result.PageSize);
        AddSasToken(resultDto.Items);
        return new OkObjectResult(resultDto);
    }

    private void AddSasToken(SellCardCategoryDto item) {
        var fileService = new FileService(_blobServiceClient);
        if(!string.IsNullOrEmpty(item.Img)) AddSasToken(fileService, item);
    }

    private void AddSasToken(List<SellCardCategoryDto> items) {
        var fileService = new FileService(_blobServiceClient);
        foreach(var item in items) {
            if(!string.IsNullOrEmpty(item.Img)) AddSasToken(fileService, item);
        }
    }

    private void AddSasToken(FileService fileService, SellCardCategoryDto item) {
        item.ImgUrl = fileService.GetSasToken(_containerImageName, item.Img ?? "");
    }
}

public class SellCardCategoryDto : SellCardCategory {
    public string ImgUrl { get; set; } = "";
}