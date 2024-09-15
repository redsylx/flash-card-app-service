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
using Main.Models;

namespace Main.Controllers;

public class CartDetailController : ControllerBase<CartDetailController> {
    private readonly IMapper _mapper;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerImageName;
    public CartDetailController(Context context, ILogger<CartDetailController> logger, BlobServiceClient blobServiceClient) : base(context, logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerImageName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var config = new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<SellCardCategory, SellCardCategoryDto>();
            cfg.CreateMap<CartDetail, CartDetailDto>()
            .ForMember(p => p.SellCategory, p => p.MapFrom(p => p.SellCardCategory))
            .ForMember(p => p.SellCardCategory, p => p.Ignore());
        });
        _mapper = config.CreateMapper();
    }

    [HttpPost]
    public IActionResult Post([FromQuery] string cartId, [FromQuery] string sellCardCategoryId) {
        var cartDetailService = new CartDetailService(_context);
        cartDetailService.AddCartDetail(cartId, sellCardCategoryId);
        new CartService(_context).CalculateNItems(cartId);
        return new OkResult();
    }

    [HttpDelete]
    public IActionResult Delete([FromQuery] string cartId, [FromQuery] string sellCardCategoryId) {
        var cartDetailService = new CartDetailService(_context);
        cartDetailService.Remove(cartId, sellCardCategoryId);
        new CartService(_context).CalculateNItems(cartId);
        return new OkResult();
    }

    [HttpGet]
    public IActionResult ListByAccount([FromQuery] string cartId) {
        var cartDetailService = new CartDetailService(_context);
        var result = cartDetailService.GetCartDetailsByCartId(cartId);
        var listDto = _mapper.Map<List<CartDetailDto>>(result);
        AddSasToken(listDto.Select(p => p.SellCategory).ToList());
        return new OkObjectResult(listDto.Select(p => p.SellCategory).ToList());
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

public class CartDetailDto : CartDetail {
    public SellCardCategoryDto SellCategory { get; set; } = new();
}