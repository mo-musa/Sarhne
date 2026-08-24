using Hangfire;
using Sarhne.API;
using Sarhne.API.Hubs;
using Sarhne.Application.Contracts.Services.BackgroundJobs;
using Sarhne.Infrastructure.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(
        context.Configuration));

// Services
builder.Services.AddDependencies(
    builder.Configuration);


//_________________________________________________________________________________

var app = builder.Build();

// Serilog
app.UseSerilogRequestLogging();

// Background Jobs
var recurringJobManager =
    app.Services.GetRequiredService<
        IRecurringJobManager>();

recurringJobManager.AddOrUpdate<ICleanupJob>(
    "cleanup-old-data",
    job => job.ExecuteAsync(
        CancellationToken.None),
    Cron.Daily);

// Identity Seeding
using (var scope = app.Services.CreateScope())
{
    var seeder =
        scope.ServiceProvider
            .GetRequiredService<IdentitySeeder>();

    await seeder.SeedAsync();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Exception Handling
app.UseExceptionHandler();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseHangfireDashboard("/hangfire");
app.MapHealthChecks("/health");

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<MessageHub>("/hubs/messages");

app.Run();
