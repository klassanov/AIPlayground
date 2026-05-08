namespace AIPlayground.Console.Config;

public class SemanticKernelSettings
{
    public ModelProviderSettings AzureOpenAI { get; set; } = new();
    public ModelProviderSettings OpenAI { get; set; } = new();
    public ModelProviderSettings AzureAIInference { get; set; } = new();
    public ModelProviderSettings GitHub { get; set; } = new();
    public ModelProviderSettings HuggingFace { get; set; } = new();
}

public class ModelProviderSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
