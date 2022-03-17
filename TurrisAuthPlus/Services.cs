namespace TurrisAuthPlus;

public class Services
{
    public readonly GameCodesService gameCodes;
    public readonly AccountsService accounts;
    public readonly ServersService servers;
    public readonly PlayersService players;
    public readonly AuthService auth;

    public Services()
    {
        gameCodes = new(this);
        accounts = new(this);
        servers = new(this);
        players = new(this);
        auth = new(this);
    }
}