namespace TurrisAuth;

public class TurrisAuthData
{
    public string ServerKey { get; private set; }
    public string ClientKey { get; private set; }

    public string ServerKeyPath => Directory.GetCurrentDirectory() + "/serverKey.secret";
    public string ClientKeyPath => Directory.GetCurrentDirectory() + "/clientKey.secret";
    public string GameCodesPath => Directory.GetCurrentDirectory() + "/gameCodes.turris";
    public string AccountsPath => Directory.GetCurrentDirectory() + "/accounts.turris";

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
        (ServerKey, ClientKey) = LoadKeys();
    }

    #region DataManagement
    public void AddGameCode(string gameCode)
    {
        lock (gameCodesLock)
        {
            GameCodes.Add(gameCode);
        }
    }
    public void RemoveExpiredPlayers()
    {
        lock (playersLock)
        {
            DateTime now = DateTime.Now;
            Players.FindAll(player => now >= player.expiration)
                .ForEach(expiredPlayer => RemovePlayer(expiredPlayer));
        }
    }
    #endregion DataManagement

    #region PlayerManagement
    public void AddPlayer(TurrisPlayer player)
    {
        lock (playersLock)
        {
            Players.Add(player);
        }
        PlayerCache.TryAdd(player.username, player);
        PlayerCache.TryAdd(player.authToken, player);
    }
    public void RemovePlayer(string username)
    {
        if (PlayerCache.TryRemove(username, out TurrisPlayer? player))
        {
            PlayerCache.TryRemove(player.authToken, out _);
            lock (playersLock)
            {
                Players.Remove(player);
            }
        }
    }
    public void RemovePlayer(TurrisPlayer player)
    {
        Players.Remove(player);
        PlayerCache.TryRemove(player.username, out _);
        PlayerCache.TryRemove(player.authToken, out _);
    }
    public bool GetPlayer(string authToken, out TurrisPlayer? player)
    {
        return PlayerCache.TryGetValue(authToken, out player);
    }
    #endregion PlayerManagement

    #region AccountManagement
    public void AddAccount(string username, string passwordHash)
    {
        lock (accountsLock)
            Accounts.Add(username, passwordHash);
    }
    public void RemoveAccount(string username)
    {
        lock (accountsLock)
            Accounts.Remove(username);
    }
    public bool GetPasswordHash(string username, out string? passwordHash)
    {
        bool accountExists;
        lock (accountsLock)
        {
            accountExists = Accounts.TryGetValue(username, out passwordHash);
        }
        return accountExists;
    }
    #endregion AccountManagement

    #region SavingAndLoading
    public void QueueSave() => queuedSaveAccounts = queuedSaveGameCodes = true;
    public void QueueGameCodesSave() => queuedSaveGameCodes = true;
    public void QueueAccountsSave() => queuedSaveAccounts = true;

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
                accountsCopy = new(Accounts);
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
        if (File.Exists(GameCodesPath))
        {
            GameCodes.Clear();
            GameCodes.AddRange(await File.ReadAllLinesAsync(GameCodesPath));
        }
        if (File.Exists(AccountsPath))
        {
            Accounts.Clear();
            string[] accountLines = await File.ReadAllLinesAsync(AccountsPath);
            foreach (string accountLine in accountLines)
            {
                string[] account = accountLine.Split(':');
                Accounts.Add(account[0], account[1]);
            }
        }
    }

    private (string, string) LoadKeys()
    {
        string serverKey;
        string clientKey;
        if (!File.Exists(ServerKeyPath))
        {
            Console.WriteLine($"[TurrisAuthData] server key not found, creating file with new key");
            serverKey = Guid.NewGuid().ToString();
            File.WriteAllText(ServerKeyPath, serverKey);
        }
        else
            serverKey = File.ReadAllText(ServerKeyPath);
        if (!File.Exists(ClientKeyPath))
        {
            Console.WriteLine($"[TurrisAuthData] client key not found, creating file with new key");
            clientKey = Guid.NewGuid().ToString();
            File.WriteAllText(ClientKeyPath, clientKey);
        }
        else
            clientKey = File.ReadAllText(ClientKeyPath);
        return (serverKey, clientKey);
    }
    #endregion SavingAndLoading
}