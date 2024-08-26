using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Main.Consts;
using Main.Exceptions;
using Main.Utils;

namespace Main.Services;

public class FileService {
    private readonly BlobServiceClient _blobServiceClient;
    private readonly List<string> allowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif"];

    public FileService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }
    
    public string GetSasToken(string container, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);
        var sasToken = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
        return sasToken.ToString();
    }

    public UploadResult UploadImage(string fileName) {
        var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

        if (!allowedImageExtensions.Any(p => p == fileExtension)) throw new BadRequestException("Invalid file extension. Only image files are allowed.");

        string containerName = Environment.GetEnvironmentVariable(EnvironmentVariables.CONTAINER_NAME_IMAGE) ?? "";
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        string blobName = $"{Guid.NewGuid()}{fileExtension}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var maxFileSize = 1 * 1024 * 1024;

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15) // Expiry 15 menit
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        var accountKey = Environment.GetEnvironmentVariable(EnvironmentVariables.STORAGE_ACCOUNT_KEY);
        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobServiceClient.AccountName, accountKey)).ToString();
        var sasUrl = $"{blobClient.Uri}?{sasToken}";

        return new UploadResult {
            FileName = blobName,
            UploadUrl = sasUrl,
            MaxSize = maxFileSize
        };
    }
}