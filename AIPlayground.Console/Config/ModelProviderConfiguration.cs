using Microsoft.SemanticKernel;

namespace AIPlayground.Console.Config
{
    enum ModelHost
    {
        AZURE_OPENAI,
        OPENAI,
        AZURE_AI_INFERENCE,
        GITHUB
    }

    internal static class ModelProviderConfiguration
    {
        extension(IKernelBuilder builder)
        {
            //internal void ConfigureModel(ModelType modelType, SemanticKernelSettings settings)
            //{
            //    switch (modelType)
            //    {
            //        case ModelType.AZURE_OPENAI:
            //            builder.AddAzureOpenAIChatCompletion(
            //                deploymentName: settings.AzureOpenAI.DeploymentName,
            //                endpoint: settings.AzureOpenAI.Endpoint,
            //                apiKey: settings.AzureOpenAI.ApiKey);

            //            break;

            //        case ModelType.OPENAI:
            //            builder.AddOpenAIChatCompletion(
            //                modelId: settings.OpenAI.ModelId,
            //                apiKey: settings.OpenAI.ApiKey);

            //            break;
            //    }
            //}

            internal void ConfigureModels(SemanticKernelSettings settings)
            {
                builder.AddAzureOpenAIChatCompletion(
                            deploymentName: settings.AzureOpenAI.DeploymentName,
                            endpoint: settings.AzureOpenAI.Endpoint,
                            apiKey: settings.AzureOpenAI.ApiKey, 
                            serviceId: ModelHost.AZURE_OPENAI.ToString());

                builder.AddOpenAIChatCompletion(
                            modelId: settings.OpenAI.ModelId,
                            apiKey: settings.OpenAI.ApiKey,
                            serviceId: ModelHost.OPENAI.ToString());

                builder.AddAzureAIInferenceChatCompletion(
                            modelId: settings.AzureAIInference.ModelId,
                            endpoint: new Uri(settings.AzureAIInference.Endpoint),
                            apiKey: settings.AzureAIInference.ApiKey,
                            serviceId: ModelHost.AZURE_AI_INFERENCE.ToString());

                builder.AddAzureAIInferenceChatCompletion(
                           modelId: settings.GitHub.ModelId,
                           endpoint: new Uri(settings.GitHub.Endpoint),
                           apiKey: settings.GitHub.ApiKey,
                           serviceId: ModelHost.GITHUB.ToString());
            }
        }

        //public static void ConfigureModel(this IKernelBuilder builder, ModelType modelType, SemanticKernelSettings settings)
        //{
        //    switch (modelType)
        //    {
        //        case ModelType.AZURE_OPENAI:
        //            builder.AddAzureOpenAIChatCompletion(
        //                deploymentName: settings.AzureOpenAI.DeploymentName,
        //                endpoint: settings.AzureOpenAI.Endpoint,
        //                apiKey: settings.AzureOpenAI.ApiKey);

        //            break;

        //        case ModelType.OPENAI:
        //            builder.AddOpenAIChatCompletion(
        //                modelId:settings.OpenAI.ModelId,
        //                apiKey: settings.OpenAI.ApiKey);

        //            break;
        //    }
        //}


    }
}
