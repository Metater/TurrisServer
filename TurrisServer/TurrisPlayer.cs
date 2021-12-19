namespace TurrisServer;


public class TurrisPlayer
{
    public readonly NetPeer peer;

    public TurrisPlayer(NetPeer peer)
    {
        this.peer = peer;
    }
}