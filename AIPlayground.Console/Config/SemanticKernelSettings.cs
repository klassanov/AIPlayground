namespace AIPlayground.Console.Config;

public class SemanticKernelSettings
{
    public ModelProviderSettings AzureOpenAI { get; set; } = new();
    public ModelProviderSettings OpenAI { get; set; } = new();
    public ModelProviderSettings AzureAIInference { get; set; } = new();
    public ModelProviderSettings GitHub { get; set; } = new();
    public ModelProviderSettings HuggingFace { get; set; } = new();
    public LocalModelProviderSettings LocalPhi3 { get; set; } = new();
    public ModelProviderSettings Ollama { get; set; } = new();
    public ModelProviderSettings LocalLMStudioGemma { get; set; } = new();
}

public class ModelProviderSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}

public class LocalModelProviderSettings
{
    public string ModelId { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}