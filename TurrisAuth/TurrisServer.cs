namespace TurrisAuth;

public class TurrisServer
{
    public readonly string guid;
    public readonly string endpoint;

    public int loadFactor = 0;
    public int players = 0;

    public List<string> games = new();

    public TurrisServer(string guid, string endpoint)
    {
        this.guid = guid;
        this.endpoint = endpoint;
    }

}