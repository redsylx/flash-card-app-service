using Azure.Storage.Blobs;
using Main.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

public class UploadController : ControllerCore {
    private readonly BlobServiceClient _blobServiceClient;
    public UploadController(BlobServiceClient blobServiceClient) : base()
    {
        _blobServiceClient = blobServiceClient;
    }

    [HttpGet("image")]
    public IActionResult UploadUrl([FromQuery] string fileName)
    {
        var uploadService = new FileService(_blobServiceClient);
        return new OkObjectResult(uploadService.UploadImage(fileName));
    }
}