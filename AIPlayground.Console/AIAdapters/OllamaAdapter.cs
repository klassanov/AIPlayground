using AIPlayground.Console.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPlayground.Console.AIAdapters
{
    internal class OllamaAdapter : ResponseStreamingIAAdapter
    {
        public OllamaAdapter([FromKeyedServices(ModelHost.OLLAMA)] IChatCompletionService chatCompletionService)
            : base(chatCompletionService)
        {
        }
    }
}
