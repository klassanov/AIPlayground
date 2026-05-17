using AIPlayground.Console.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPlayground.Console.AIAdapters
{
    internal class GithubAdapter : ResponseStreamingIAAdapter
    {
        public GithubAdapter([FromKeyedServices(ModelHost.GITHUB)] IChatCompletionService chatCompletionService) 
            : base(chatCompletionService)
        {
        }
    }
}
