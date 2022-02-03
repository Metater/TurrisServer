namespace TurrisAuth;

public class TurrisValidation
{
    private TurrisAuthData authData;

    public TurrisValidation(TurrisAuthData authData)
    {
        this.authData = authData;
    }

    public async Task<(bool valid, TurrisPlayer? player)> ValidateAuthToken()
    {
        if (!await ctx.QueryExists("authToken")) return (false, null);
        if (!authData.PlayerCache.TryGetValue(ctx.GetQuery("authToken"), out TurrisPlayer? player))
        {
            await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
            return (false, null);
        }
        return (true, player);
    }

    public async Task<(bool valid, TurrisServer? server)> ValidateJoinCode()
    {
        if (!await ctx.QueryExists("joinCode")) return (false, null);
        if (!authData.ServerCache.TryGetValue(ctx.GetQuery("joinCode"), out TurrisServer? server))
        {
            await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
            return (false, null);
        }
        return (true, server);
    }

    // move the rest of validation logic here
}