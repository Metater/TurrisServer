namespace TurrisAuthPlus;

public class AuthService
{
    public readonly static Dictionary<string, List<string>> Endpoints = new()
    {
        { "/client/accounts/create", new List<string>() { "key", "gameCode", "username", "password" } }
    };

    private static string ClientKeysPath => Directory.GetCurrentDirectory() + "/clientKeys.secret";
    private static string ServerKeysPath => Directory.GetCurrentDirectory() + "/serverKeys.secret";

    private readonly List<Guid> clientKeys = new();
    private readonly List<Guid> serverKeys = new();

    public AuthService()
    {
        LoadKeys(clientKeys, ClientKeysPath);
        LoadKeys(serverKeys, ServerKeysPath);
    }

    public bool AuthClient(Guid key) =>
        clientKeys.Contains(key);
    public bool AuthServer(Guid key) =>
        serverKeys.Contains(key);

    private static void LoadKeys(List<Guid> keys, string keysPath)
    {
        if (!File.Exists(keysPath))
        {
            Console.WriteLine($"[AuthData] creating a new key at, no key found at: {keysPath}");
            Guid key = Guid.NewGuid();
            File.WriteAllText(keysPath, key.ToString());
            keys.Add(key);
            return;
        }
        string[] loadedKeys = File.ReadAllLines(keysPath);
        foreach (string loadedKey in loadedKeys)
            keys.Add(Guid.Parse(loadedKey));
    }
}
