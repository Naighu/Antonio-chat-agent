namespace Antonio.Cli;

// Saves the session id to ~/.antonio/session so Antonio remembers you between runs.
public static class SessionFile
{
    private static readonly string Dir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".antonio");

    private static readonly string FilePath = Path.Combine(Dir, "session");

    public static string LoadOrCreate()
    {
        if (File.Exists(FilePath))
        {
            var id = File.ReadAllText(FilePath).Trim();
            if (id.Length > 0)
                return id;
        }
        return CreateNew();
    }

    public static string CreateNew()
    {
        Directory.CreateDirectory(Dir);                 
        var id = Guid.NewGuid().ToString("N"); 
        File.WriteAllText(FilePath, id);
        return id;
    }
}