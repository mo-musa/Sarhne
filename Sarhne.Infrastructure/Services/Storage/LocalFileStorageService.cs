using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sarhne.Application.Contracts.Services.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Services.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(
        IWebHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken)
    {
        var uploadsPath = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            folder);

        Directory.CreateDirectory(uploadsPath);

        var extension = Path.GetExtension(fileName);

        var storedFileName =
            $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(
            uploadsPath,
            storedFileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        var request = _httpContextAccessor.HttpContext?.Request;

        var baseUrl =
            $"{request?.Scheme}://{request?.Host}";

        return $"{baseUrl}/uploads/{folder}/{storedFileName}";
    }

    public Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return Task.CompletedTask;

        var uri = new Uri(fileUrl);

        var relativePath = uri.AbsolutePath
            .TrimStart('/')
            .Replace(
                '/',
                Path.DirectorySeparatorChar);

        var filePath = Path.Combine(
            _environment.WebRootPath,
            relativePath);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}