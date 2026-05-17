using Microsoft.SemanticKernel;

namespace AIPlayground.Console.Config
{
    class ModelHost
    {
        public const string AZURE_OPENAI = "AZURE_OPENAI";
        public const string OPENAI = "OPENAI";
        public const string AZURE_AI_INFERENCE = "AZURE_AI_INFERENCE";
        public const string GITHUB = "GITHUB";
        public const string HUGGING_FACE = "HUGGING_FACE";
        public const string LOCAL_PHI3 = "LOCAL_PHI3";
    }

    internal static class ModelProviderConfiguration
    {
        extension(IKernelBuilder builder)
        {
            internal void ConfigureModels(SemanticKernelSettings settings)
            {
                builder.AddAzureOpenAIChatCompletion(
                            deploymentName: settings.AzureOpenAI.ModelId,
                            endpoint: settings.AzureOpenAI.Endpoint,
                            apiKey: settings.AzureOpenAI.ApiKey,
                            serviceId: ModelHost.AZURE_OPENAI);

                builder.AddOpenAIChatCompletion(
                            modelId: settings.OpenAI.ModelId,
                            apiKey: settings.OpenAI.ApiKey,
                            serviceId: ModelHost.OPENAI);

                builder.AddAzureAIInferenceChatCompletion(
                            modelId: settings.AzureAIInference.ModelId,
                            endpoint: new Uri(settings.AzureAIInference.Endpoint),
                            apiKey: settings.AzureAIInference.ApiKey,
                            serviceId: ModelHost.AZURE_AI_INFERENCE);

                builder.AddAzureAIInferenceChatCompletion(
                           modelId: settings.GitHub.ModelId,
                           endpoint: new Uri(settings.GitHub.Endpoint),
                           apiKey: settings.GitHub.ApiKey,
                           serviceId: ModelHost.GITHUB);

                builder.AddHuggingFaceChatCompletion(
                           model: settings.HuggingFace.ModelId,
                           endpoint: new Uri(settings.HuggingFace.Endpoint),
                           apiKey: settings.HuggingFace.ApiKey,
                           serviceId: ModelHost.HUGGING_FACE);

                builder.AddOnnxRuntimeGenAIChatCompletion(
                            modelId: settings.LocalPhi3.ModelId,
                            modelPath: settings.LocalPhi3.ModelPath,
                            serviceId: ModelHost.LOCAL_PHI3);
            }
        }
    }
}
