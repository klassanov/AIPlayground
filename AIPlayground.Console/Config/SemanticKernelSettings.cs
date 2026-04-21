namespace AIPlayground.Console.Config;

public class SemanticKernelSettings
{
    public AzureOpenAISettings AzureOpenAI { get; set; } = new();
}

public class AzureOpenAISettings
{
    public string DeploymentName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
