using AzureFrameworkPOC.AI;
using AzureFrameworkPOC.Console;
using AzureFrameworkPOC.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder =
    Host.CreateApplicationBuilder(args);

builder.Services.AddAI(builder.Configuration);
builder.Services.AddRepositoryExploration();

builder.Services.AddTransient<ConsoleApplication>();

using IHost host = builder.Build();

using var cancellationTokenSource =
    new CancellationTokenSource();

System.Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};

try
{
    ConsoleApplication application =
        host.Services.GetRequiredService<
            ConsoleApplication>();

    await application.RunAsync(
        args,
        cancellationTokenSource.Token);
}
catch (OperationCanceledException)
{
    System.Console.WriteLine(
        "Operation cancelled.");
}
catch (Exception exception)
{
    System.Console.Error.WriteLine(
        $"Application failed: {exception.Message}");

    Environment.ExitCode = 1;
}