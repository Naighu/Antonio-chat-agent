using Antonio.Cli;

var baseUrl = Environment.GetEnvironmentVariable("ANTONIO_URL") ?? "http://127.0.0.1:5249/";

using var http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(90) };
var api = new AntonioApiClient(http);
var sessionId = SessionFile.LoadOrCreate();

// Ctrl+C cancels the current request instead of killing the app
CancellationTokenSource? current = null;
Console.CancelKeyPress += (_, e) =>
{
    if (current is not null)
    {
        e.Cancel = true;        // keep the app running
        current.Cancel();       // abort the HTTP call (and the Claude call behind it)
    }
};

// One-shot mode: dotnet run --project src/Antonio.Cli -- "what's the time in Brisbane?"
if (args.Length > 0)
{
    await AskAsync(string.Join(' ', args));
    return;
}

// Interactive mode
Console.WriteLine("Antonio is ready. Type a message, /new for a new conversation, /exit to quit.");

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine()?.Trim();

    if (line is null || line == "/exit")       // null = Ctrl+D (Linux/macOS) or Ctrl+Z (Windows)
        break;

    if (line.Length == 0)
        continue;

    if (line == "/new")
    {
        sessionId = SessionFile.CreateNew();
        Console.WriteLine("Started a new conversation.");
        continue;
    }

    await AskAsync(line);
}

async Task AskAsync(string message)
{
    current = new CancellationTokenSource();
    try
    {
        var reply = await api.SendAsync(sessionId, message, current.Token);
        Console.WriteLine($"Antonio: {reply}");
    }
    catch (OperationCanceledException) when (current.IsCancellationRequested)
    {
        Console.WriteLine("(cancelled)");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("(timed out, no reply from Antonio)");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine("Is the API running? Start it with: dotnet run --project src/Antonio.Api");
    }
    finally
    {
        current.Dispose();
        current = null;
    }
}