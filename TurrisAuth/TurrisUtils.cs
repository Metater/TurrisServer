namespace TurrisAuth;

public static class TurrisUtils
{
    public static async Task<bool> KeyValid(HttpContext ctx, string key)
    {
        if (!await QueryValid(ctx, "key")) return false;
        if (GetQuery(ctx, "key") != key)
        {
            await ctx.Response.WriteAsync("400\nInvalidKey");
            return false;
        }
        return true;
    }

    public static async Task<bool> QueryValid(HttpContext ctx, string query)
    {
        bool hasQuery = ctx.Request.Query.ContainsKey(query);
        if (!hasQuery) await ctx.Response.WriteAsync($"400\nMissingQuery:{query}");
        return hasQuery;
    }

    public static string GetQuery(HttpContext ctx, string query)
    {
        return ctx.Request.Query[query];
    }

    public static string Hash(string data)
    {
        HashAlgorithm sha = SHA256.Create();
        byte[] result = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
        return Convert.ToBase64String(result);
    }




}