using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureFrameworkPOC.AI.Providers.FoundryLocal;

public sealed class FoundryLocalRuntime :
    IFoundryLocalRuntime,
    IAsyncDisposable
{
    private readonly FoundryLocalOptions _options;
    private readonly ILogger<FoundryLocalRuntime> _logger;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    private IModel? _model;
    private bool _started;
    private bool _disposed;

    public FoundryLocalRuntime(
        IOptions<FoundryLocalOptions> options,
        ILogger<FoundryLocalRuntime> logger)
    {
        _options = options.Value;
        _logger = logger;

        OpenAiEndpoint = new Uri(
            $"{_options.Endpoint.TrimEnd('/')}/v1");
    }

    public string ModelId =>
        _model?.Id
        ?? throw new InvalidOperationException(
            "Foundry Local has not been started.");

    public Uri OpenAiEndpoint { get; }

    public async Task EnsureStartedAsync(
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_started)
        {
            return;
        }

        await _initializationLock.WaitAsync(cancellationToken);

        try
        {
            if (_started)
            {
                return;
            }

            await StartCoreAsync(cancellationToken);
            _started = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private async Task StartCoreAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Initializing Foundry Local at {Endpoint}.",
            _options.Endpoint);

        var configuration = new Configuration
        {
            AppName = "AzureFrameworkPOC",
            LogLevel =
                Microsoft.AI.Foundry.Local.LogLevel.Information,

            Web = new Configuration.WebService
            {
                Urls = _options.Endpoint
            }
        };

        await FoundryLocalManager.CreateAsync(
            configuration,
            _logger);

        FoundryLocalManager manager =
            FoundryLocalManager.Instance;

        if (_options.DownloadExecutionProviders)
        {
            await DownloadExecutionProvidersAsync(
                manager,
                cancellationToken);
        }

        var catalog = await manager.GetCatalogAsync();

        _model = await catalog.GetModelAsync(
            _options.ModelAlias)
            ?? throw new InvalidOperationException(
                $"Foundry Local model '{_options.ModelAlias}' " +
                "was not found in the catalog.");

        _model = _model.Variants
            .FirstOrDefault(v => v.Id.Contains("generic-cpu"))
            ?? throw new InvalidOperationException(
                $"was not found for model '{_options.ModelAlias}'.");

        _logger.LogInformation(
            "Ensuring model {ModelAlias} is downloaded.",
            _options.ModelAlias);

        if (!await _model.IsCachedAsync(cancellationToken))
        {
            await _model.DownloadAsync(
            progress =>
            {
                _logger.LogInformation(
                    "Model download progress: {Progress:F1}%",
                    progress);
            },
            cancellationToken);
        }
        else
        {
            _logger.LogInformation(
                "Model {ModelAlias} is already cached.",
                _options.ModelAlias);
        }

        await _model.LoadAsync(cancellationToken);

        _logger.LogInformation(
            "Starting Foundry Local OpenAI-compatible service.");

        await manager.StartWebServiceAsync(
            cancellationToken);

        _logger.LogInformation(
            "Foundry Local is ready. Model ID: {ModelId}. " +
            "OpenAI endpoint: {Endpoint}.",
            _model.Id,
            OpenAiEndpoint);
    }

    private async Task DownloadExecutionProvidersAsync(
        FoundryLocalManager manager,
        CancellationToken cancellationToken)
    {
        string? currentExecutionProvider = null;

        await manager.DownloadAndRegisterEpsAsync(
            (executionProviderName, percentage) =>
            {
                if (!string.Equals(
                        currentExecutionProvider,
                        executionProviderName,
                        StringComparison.Ordinal))
                {
                    currentExecutionProvider =
                        executionProviderName;

                    _logger.LogInformation(
                        "Preparing execution provider {Provider}.",
                        executionProviderName);
                }

                _logger.LogDebug(
                    "Execution provider {Provider}: {Percentage:F1}%",
                    executionProviderName,
                    percentage);
            },
            cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            if (_started)
            {
                await FoundryLocalManager.Instance
                    .StopWebServiceAsync();
            }

            if (_model is not null &&
                _options.UnloadModelOnShutdown)
            {
                await _model.UnloadAsync();
            }
        }
        finally
        {
            _initializationLock.Dispose();
        }
    }
}