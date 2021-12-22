namespace TurrisServer;

public class TurrisServer : INetEventListener
{
    public readonly Stopwatch timer = Stopwatch.StartNew();
    public readonly TurrisDispatcher dispatcher;
    public int pollPeriod;
    public readonly int port;
    public NetManager server;

    private readonly Stopwatch sendUpdateTimer = Stopwatch.StartNew();

    private List<TurrisGame> games = new();

    private Dictionary<int, NetPeer> players = new();

    public TurrisServer(TurrisDispatcher dispatcher, int pollPeriod, int port)
    {
        this.dispatcher = dispatcher;
        this.pollPeriod = pollPeriod;
        this.port = port;
        server = new NetManager(this);
        server.StartInManualMode(port);
        Console.WriteLine($"[TurrisServer] Started server on port {port}");
    }

    public void Tick()
    {
        if (sendUpdateTimer.ElapsedMilliseconds >= 10000)
        {
            sendUpdateTimer.Restart();
            dispatcher.Dispatch(async () =>
            {
                List<string> lines = new();
                _ = await Program.HttpClient.PostAsync("https://auth.turris.ml:2096/update", new StringContent(string.Join('\n', lines)));
            });
        }
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        if (server.ConnectedPeersCount < 100)
        {
            //NetPeer client = request.AcceptIfKey()
            //client.Id
            Console.WriteLine(request.Data.GetString());
        }
        else
            request.Reject();
    }
    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {

    }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {

    }
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {

    }
    public void OnPeerConnected(NetPeer peer)
    {

    }
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {

    }
}