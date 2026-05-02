using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace AIPlayground.Console.Config
{
    enum ModelType
    {
        AZURE_OPENAI,
        OPENAI
    }

    internal static class ModelProviderConfiguration
    {
        public static void ConfigureModel(this IKernelBuilder builder, ModelType modelType, SemanticKernelSettings settings)
        {
            switch (modelType)
            {
                case ModelType.AZURE_OPENAI:
                    builder.AddAzureOpenAIChatCompletion(
                        deploymentName: settings.AzureOpenAI.DeploymentName,
                        endpoint: settings.AzureOpenAI.Endpoint,
                        apiKey: settings.AzureOpenAI.ApiKey);

                    break;

                case ModelType.OPENAI:
                    builder.AddOpenAIChatCompletion(
                        modelId:settings.OpenAI.ModelId,
                        apiKey: settings.OpenAI.ApiKey);

                    break;
            }
        }
    }
}
