namespace TurrisAuth;

public class TurrisServer
{
    public readonly string guid;
    public readonly string ep;

    public int loadFactor = 0;
    public int players = 0;

    public List<string> games = new();

    public TurrisServer(string guid, string ep)
    {
        this.guid = guid;
        this.ep = ep;
    }

}