using ProductApi.Models.AI;

namespace ProductApi.Contracts
{
    public interface IAIService
    {
        Task<AIAskResponse> AskAsync(AIAskRequest request);
    }
}
