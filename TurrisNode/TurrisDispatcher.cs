namespace TurrisServer;

public class TurrisDispatcher
{
    private Channel<Action> channel;

    public readonly string serverKey;

    public readonly Task task;

    public TurrisDispatcher(string serverKey)
    {
        this.serverKey = serverKey;

        channel = Channel.CreateUnbounded<Action>(new UnboundedChannelOptions() { SingleReader = true });
        task = Task.Run(async () =>
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