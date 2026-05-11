using AIPlayground.Console.Config;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPlayground.Console.AIHostingAdapters
{
    internal class GithubAdapter : HostingAdapter
    {
        public GithubAdapter([FromKernelServices(ModelHost.GITHUB)]IChatCompletionService chatCompletionService) 
            : base(chatCompletionService)
        {
        }
    }
}
