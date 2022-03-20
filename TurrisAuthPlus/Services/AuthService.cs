namespace TurrisAuthPlus.Services;

public class AuthService
{
    private TurrisServices services;

    public readonly static Dictionary<string, List<string>> Endpoints = new()
    {
        { "/client/createaccount", new List<string>() { "ckey", "gameCode", "username", "password" } },
        { "/client/authplayer", new List<string>() { "ckey", "username", "password" } }
    };

    private static string ClientKeysPath = Directory.GetCurrentDirectory() + "/clientKeys.secret";
    private static string ServerKeysPath = Directory.GetCurrentDirectory() + "/serverKeys.secret";

    private static readonly List<Guid> clientKeys = new();
    private static readonly List<Guid> serverKeys = new();

    public AuthService(TurrisServices services)
    {
        this.services = services;
        LoadKeys(clientKeys, ClientKeysPath);
        LoadKeys(serverKeys, ServerKeysPath);
    }

    public static bool AuthClient(Guid key) =>
        clientKeys.Contains(key);
    public static bool AuthServer(Guid key) =>
        serverKeys.Contains(key);

    private static void LoadKeys(List<Guid> keys, string keysPath)
    {
        keys.Clear();
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
