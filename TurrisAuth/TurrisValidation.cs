namespace TurrisAuth;

public class TurrisValidation
{
    private readonly TurrisAuthData authData;

    public TurrisValidation(TurrisAuthData authData)
    {
        this.authData = authData;
    }

    public async Task<bool> AuthServer(HttpContext ctx) =>
        await ctx.ValidateKey(key => key == authData.ServerKey);
    public async Task<bool> AuthClient(HttpContext ctx) =>
        await ctx.ValidateKey(key => key == authData.ClientKey);

    public async Task<(bool valid, TurrisPlayer? player)> AuthToken(HttpContext ctx)
    {
        if (!await ctx.QueryExists("authToken")) return (false, null);
        if (!authData.GetPlayer(ctx.GetQuery("authToken"), out TurrisPlayer? player))
        {
            await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
            return (false, null);
        }
        return (true, player);
    }

    public async Task<(bool valid, TurrisServer? server)> JoinCode(HttpContext ctx)
    {
        if (!await ctx.QueryExists("joinCode")) return (false, null);
        if (!authData.ServerCache.TryGetValue(ctx.GetQuery("joinCode"), out TurrisServer? server))
        {
            await ctx.Response.WriteAsync("400\nJoinCodeInvalid");
            return (false, null);
        }
        return (true, server);
    }

    public async Task<(bool valid, string gameCode)> GameCode(HttpContext ctx)
    {
        if(!await ctx.QueryExists("gameCode")) return (false, "");
        string gameCode = ctx.GetQuery("gameCode");
        bool gameCodeValid;
        lock (authData.gameCodesLock)
        {
            gameCodeValid = authData.GameCodes.Remove(gameCode);
        }
        if (!gameCodeValid)
        {
            await ctx.Response.WriteAsync("400\nGameCodeInvalid");
            return (false, "");
        }
        return (true, gameCode);
    }

    public async Task<(bool valid, string password)> Password(HttpContext ctx)
    {
        if (!await ctx.QueryExists("password")) return (false, "");
        string password = ctx.GetQuery("password");
        if (!PasswordValid(password))
        {
            await ctx.Response.WriteAsync("400\nPasswordInvalid");
            return (false, "");
        }
        return (true, password);
    }

    public async Task<(bool valid, string username)> Username(HttpContext ctx)
    {
        if (!await ctx.QueryExists("username")) return (false, "");
        string username = ctx.GetQuery("username");
        if (!UsernameValid(username))
        {
            await ctx.Response.WriteAsync("400\nUsernameInvalid");
            return (false, "");
        }
        return (true, username);
    }

    public async Task Intent(HttpContext ctx)
    {
        if (!await ctx.QueryExists("authToken")) return;
        if (!await ctx.QueryExists("serverId")) return;
        if (!await ctx.QueryExists("serverIntentType")) return;

        string authToken = ctx.GetQuery("username");
        if (!authData.GetPlayer(authToken, out TurrisPlayer? player))
        {
            await ctx.Response.WriteAsync("400\nAuthTokenInvalid");
            return;
        }

        bool intentInvalid = DateTime.Now >= player!.serverIntentExpiration ||
            player.serverIntent != ctx.GetQuery("serverId") ||
            !int.TryParse(ctx.GetQuery("serverIntentType"), out int serverIntentType) ||
            ((int)player.serverIntentType) != serverIntentType;
        player.Reset();
        if (intentInvalid)
        {
            await ctx.Response.WriteAsync("400\nServerIntentInvalid");
            return;
        }

        await ctx.Response.WriteAsync($"200\nUsername:{player.username}");
    }

    private static bool PasswordValid(string password)
    {
        return password.Length >= 6 && password.Length <= 18;
    }

    private static bool UsernameValid(string username)
    {
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$") &&
            username.Length >= 1 &&
            username.Length <= 16;
    }
}