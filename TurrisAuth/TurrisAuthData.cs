namespace TurrisAuth;

public class TurrisAuthData
{
    public string ServerKey { get; private set; }
    public string ClientKey { get; private set; }

    public string GameCodesPath { get; private set; }
    public string AccountsPath { get; private set; }

    public object gameCodesLock = new();
    // <gameCodes>
    public List<string> GameCodes { get; private set; } = new();

    public object accountsLock = new();
    // <username, passwordHash>
    public Dictionary<string, string> Accounts { get; private set; } = new();

    public object serverLock = new();
    // <server>
    public List<TurrisServer> Servers { get; private set; } = new();
    // <>
    public ConcurrentDictionary<string, TurrisServer> ServerCache { get; private set; } = new();

    private bool queuedSaveGameCodes = false;
    private bool queuedSaveAccounts = false;

    public TurrisAuthData()
    {
        GameCodesPath = Directory.GetCurrentDirectory() + "/gameCodes.turris";
        AccountsPath = Directory.GetCurrentDirectory() + "/accounts.turris";


    }

    public async Task Load()
    {
        ServerKey = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/serverKey.secret");
        ClientKey = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/clientKey.secret");

    }
}