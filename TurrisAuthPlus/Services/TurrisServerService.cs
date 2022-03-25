namespace TurrisAuthPlus.Services;

public class TurrisServerService
{
    private readonly TurrisServices services;

    public TurrisServerService(TurrisServices services)
    {
        this.services = services;
    }

    public string DeleteAccount(string username)
    {
        services.accounts.DeleteAccount(username);
        return "200";
    }

    public string StartServer(string serverId, string endpoint)
    {
        if (!TurrisUtils.GuidLengthValid(serverId))
            return "400\nServerIdInvalid";
        if (!TurrisUtils.EndpointLengthValid(endpoint))
            return "400\nEndpointInvalid";
        services.servers.StartServer(serverId, endpoint);
        return "200";
    }
    public string StopServer(string serverId)
    {
        if (!TurrisUtils.GuidLengthValid(serverId))
            return "400\nServerIdInvalid";
        services.servers.StopServer(serverId);
        return "200";
    }
    public string PollServer(string serverId)
    {

    }
}