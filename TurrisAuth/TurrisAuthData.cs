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

    public object serversLock = new();
    // <server>
    public List<TurrisServer> Servers { get; private set; } = new();
    // <serverGuid|joinCode, server>
    public ConcurrentDictionary<string, TurrisServer> ServerCache { get; private set; } = new();

    public object playersLock = new();
    // <player>
    public List<TurrisPlayer> Players { get; private set; } = new();
    // <username|authToken, player>
    public ConcurrentDictionary<string, TurrisPlayer> PlayerCache { get; private set; } = new();

    private bool queuedSaveGameCodes = false;
    private bool queuedSaveAccounts = false;

    public TurrisAuthData()
    {
        GameCodesPath = Directory.GetCurrentDirectory() + "/gameCodes.turris";
        AccountsPath = Directory.GetCurrentDirectory() + "/accounts.turris";
    }

    public void RemoveExpiredPlayers()
    {
        lock (playersLock)
        {
            DateTime now = DateTime.Now;
            Players.FindAll(player => now >= player.expiration).ForEach(expiredPlayer =>
            {
                Players.Remove(expiredPlayer);
                PlayerCache.TryRemove(expiredPlayer.username, out _);
                PlayerCache.TryRemove(expiredPlayer.authToken, out _);
            });
        }
    }

    public void QueueGameCodesSave()
    {
        queuedSaveGameCodes = true;
    }

    public void QueueAccountsSave()
    {
        queuedSaveAccounts = true;
    }

    public async Task ForceSave()
    {
        queuedSaveGameCodes = true;
        queuedSaveAccounts = true;
        await Save();
    }

    public async Task Save()
    {
        List<Task> savingFiles = new();
        if (queuedSaveGameCodes)
        {
            queuedSaveGameCodes = false;
            List<string> gameCodesCopy;
            lock (gameCodesLock)
            {
                gameCodesCopy = new(GameCodes);
            }
            savingFiles.Add(File.WriteAllLinesAsync(GameCodesPath, gameCodesCopy));
        }
        if (queuedSaveAccounts)
        {
            queuedSaveAccounts = false;
            List<KeyValuePair<string, string>> accountsCopy;
            lock (accountsLock)
            {
                accountsCopy = new(accounts);
            }
            List<string> accountLines = new();
            foreach ((string username, string passwordHash) in accountsCopy)
            {
                accountLines.Add(username + ":" + passwordHash);
            }
            savingFiles.Add(File.WriteAllLinesAsync(AccountsPath, accountLines));
        }
        await Task.WhenAll(savingFiles);
    }

    public async Task Load()
    {
        ServerKey = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/serverKey.secret");
        ClientKey = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "/clientKey.secret");
        if (File.Exists(gameCodesPath))
            GameCodes = new(await File.ReadAllLinesAsync(GameCodesPath));
        if (File.Exists(accountsPath))
        {
            string[] accountLines = await File.ReadAllLinesAsync(AccountsPath);
            foreach (string accountLine in accountLines)
            {
                string[] account = accountLine.Split(':');
                accounts.Add(account[0], account[1]);
            }
        }
    }
}