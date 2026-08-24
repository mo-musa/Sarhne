using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Application.Contracts.Services.BackgroundJobs;

public interface ICleanupJob
{
    Task ExecuteAsync(
        CancellationToken cancellationToken = default);
}