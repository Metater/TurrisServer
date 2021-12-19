namespace TurrisServer;

public class TurrisDispatcher
{
    private Channel<Action>? channel;

    public readonly string serverKey;

    public TurrisDispatcher(string serverKey)
    {
        this.serverKey = serverKey;
    }

    public Task Start()
    {
        channel = Channel.CreateUnbounded<Action>(new UnboundedChannelOptions() { SingleReader = true });
        return Task.Run(async () =>
        {
            while (await channel.Reader.WaitToReadAsync())
            {
                while (channel.Reader.TryRead(out var action))
                {
                    action();
                }
            }
        });
    }

    public void Dispatch(Action action)
    {
        channel!.Writer.TryWrite(action);
    }

    public void Stop()
    {
        channel!.Writer.Complete();
    }
}