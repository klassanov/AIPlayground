namespace AIPlayground.Console.Config;

public class SemanticKernelSettings
{
    public AzureOpenAISettings AzureOpenAI { get; set; } = new();
    public OpenAISettings OpenAI { get; set; } = new();
    public AzureAIInferenceSettings AzureAIInference { get; set; } = new();
    public GithubSettings GitHub { get; set; } = new();
}

public class AzureOpenAISettings
{
    public string DeploymentName { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
}

public class AzureAIInferenceSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class GithubSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
