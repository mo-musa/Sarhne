using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.Storage;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken);
}