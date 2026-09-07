namespace ProductApi.Contracts;

public interface IMultiIntentAIProvider
{
    Task<string> GetMultiIntentAsync(string question);
}