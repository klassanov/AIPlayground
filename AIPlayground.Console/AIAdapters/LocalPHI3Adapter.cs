using AIPlayground.Console.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Onnx;

namespace AIPlayground.Console.AIAdapters
{
    internal class LocalPHI3Adapter : ResponseStreamingIAAdapter
    {
        public LocalPHI3Adapter([FromKeyedServices(ModelHost.LOCAL_PHI3)] IChatCompletionService chatCompletionService)
            : base(chatCompletionService)
        {
        }

        protected override void InitializePromptExecutionSettings()
        {
            this.promptExecutionSettings = new OnnxRuntimeGenAIPromptExecutionSettings()
            {
                Temperature = 0.9f,
                MaxTokens = 2000
            };
        }

        protected override void DisposeChatCompletionService()
        {
            if (chatCompletionService is IDisposable disposableService)
            {
                disposableService.Dispose();
            }
        }
    }
}
