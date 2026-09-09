namespace AzureFrameworkPOC.Core.AI;

public interface IChatClientProviderResolver
{
    IChatClientProvider Resolve(ChatProviderType providerType);
}