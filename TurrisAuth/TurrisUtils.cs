namespace TurrisAuth;

public static class TurrisUtils
{
    public static async Task<bool> ValidateKey(this HttpContext ctx, Func<string, bool> keyValid)
    {
        if (!await ctx.QueryExists("key")) return false;
        if (!keyValid(GetQuery(ctx, "key")))
        {
            await ctx.Response.WriteAsync("400\nInvalidKey");
            return false;
        }
        return true;
    }

    public static async Task<bool> QueryExists(this HttpContext ctx, string query)
    {
        bool queryExists = ctx.Request.Query.ContainsKey(query);
        if (!queryExists) await ctx.Response.WriteAsync($"400\nMissingQuery:{query}");
        return queryExists;
    }

    public static string GetQuery(this HttpContext ctx, string query)
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