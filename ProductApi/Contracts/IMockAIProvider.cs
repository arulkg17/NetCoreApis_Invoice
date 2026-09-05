namespace ProductApi.Contracts;

public interface IMockAIProvider
{
    Task<string> GetIntentAsync(string question);
}
