
namespace Antonio.Infrastructure.Llm;

public interface ILlmOptions
{
   string ApiKey { get; }
    string Model { get; }
    string BaseUrl { get; }
}
