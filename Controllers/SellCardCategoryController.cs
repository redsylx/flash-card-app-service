using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Main.Consts;
using AutoMapper;
using Azure.Storage.Blobs;
using System;
using Microsoft.AspNetCore.Authorization;

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
}