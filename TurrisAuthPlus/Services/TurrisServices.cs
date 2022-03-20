namespace TurrisAuthPlus.Services;

public class TurrisServices
{
    public readonly GameCodesService gameCodes;
    public readonly AccountsService accounts;
    public readonly ServersService servers;
    public readonly PlayersService players;
    public readonly AuthService auth;

    public readonly TurrisClientService turrisClient;
    public readonly TurrisServerService turrisServer;

    public TurrisServices()
    {
        gameCodes = new(this);
        accounts = new(this);
        servers = new(this);
        players = new(this);
        auth = new(this);

        turrisClient = new(this);
        turrisServer = new(this);
    }
}