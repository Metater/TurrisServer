namespace TurrisAuthPlus;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseEndpointAuth(this IApplicationBuilder app)
    {
        return app.Use(async (ctx, next) =>
        {
            string rawPath = ctx.Request.Path.ToString();
            string path = rawPath[..(rawPath.IndexOf("?") > 0 ? rawPath.IndexOf("?") : rawPath.Length)];
            if (AuthService.Endpoints.TryGetValue(path, out List<string>? queries)
            && ctx.Request.Query.Count == queries.Count
            && ctx.Request.Query.All(query => queries.Contains(query.Key)))
            {
                await next(ctx);
            }
            else
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync("401");
            }
        });
    }

    public static IApplicationBuilder UseKeyAuth(this IApplicationBuilder app)
    {
        return app.Use(async (ctx, next) =>
        {
            if (ctx.Request.Query.ContainsKey("skey"))
            {
                if (Guid.TryParse(ctx.Request.Query["skey"], out Guid guid) && AuthService.AuthServer(guid))
                {
                    await next(ctx);
                    return;
                }
            }
            else if (ctx.Request.Query.ContainsKey("ckey"))
            {
                if (Guid.TryParse(ctx.Request.Query["ckey"], out Guid guid) && AuthService.AuthClient(guid))
                {
                    await next(ctx);
                    return;
                }
            }

            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsync("401");
        });
    }
}
