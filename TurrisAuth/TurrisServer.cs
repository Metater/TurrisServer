namespace TurrisAuth;

public class TurrisServer
{
    public readonly Guid guid;
    public readonly string ep;

    public int loadFactor = 0;
    public int players

    public TurrisServer(Guid guid, string ep)
    {
        this.guid = guid;
        this.ep = ep;
    }

}