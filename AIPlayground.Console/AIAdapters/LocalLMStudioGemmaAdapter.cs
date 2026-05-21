using AIPlayground.Console.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPlayground.Console.AIAdapters
{
    internal class LocalLMStudioGemmaAdapter : ResponseStreamingIAAdapter
    {
        public LocalLMStudioGemmaAdapter(
            [FromKeyedServices(ModelHost.LOCAL_LM_STUDIO_GEMMA)]IChatCompletionService chatCompletionService)
            : base(chatCompletionService)
        {
        }
    }
}
