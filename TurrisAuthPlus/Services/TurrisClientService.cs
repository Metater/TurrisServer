namespace TurrisAuthPlus.Services;

public class TurrisClientService
{
    private readonly TurrisServices services;

    public TurrisClientService(TurrisServices services)
    {
        this.services = services;
    }

    public string CreateAccount(string gameCode, string username, string password)
    {
        if (!TurrisUtils.GameCodeValid(gameCode))
            return "400\nGameCodeInvalid";
        if (!TurrisUtils.UsernameValid(username))
            return "400\nUsernameInvalid";
        if (!TurrisUtils.PasswordValid(password))
            return "400\nPasswordInvalid";
        if (!services.gameCodes.TryLockGameCode(gameCode))
            return "400\nGameCodeInvalid";
        if (!services.accounts.TryCreateAccount(username, password))
            return "400\nUsernameExists";
        return "200";
    }
    public string DeleteAccount(string username, string password)
    {
        if (!TurrisUtils.UsernameValid(username))
            return "400\nUsernameInvalid";
        if (!TurrisUtils.PasswordValid(password))
            return "400\nPasswordInvalid";
        if (!services.accounts.AccountValid(username, password))
            return "400\nAccountInvalid";
        services.accounts.DeleteAccount(username);
        return "200";
    }
    public string AuthPlayer(string username, string password)
    {
        if (!TurrisUtils.UsernameValid(username))
            return "400\nUsernameInvalid";
        if (!TurrisUtils.PasswordValid(password))
            return "400\nPasswordInvalid";
        if (!services.accounts.AccountValid(username, password))
            return "400\nAccountInvalid";
        PlayerModel player = services.players.GetRenewedPlayer(username);
        return $"200\nAuthToken:{player.AuthToken}";
    }
    public string CreateGame(string authToken)
    {
        if (!TurrisUtils.AuthTokenValid(authToken))
            return "400\nAuthTokenInvalid";
        bool createdGame = false;
        PlayerModel? player = null;
        ServerModel? server = null;
        GameModel? game = null;
        if (!services.players.PlayerValid(authToken, validatedPlayer =>
        {
            player = validatedPlayer;
            createdGame = services.servers.TryCreateGame(validatedPlayer, out server, out game);
        })) return "400\nAuthTokenInvalid";
        if (!createdGame)
            return "400\nReserveServerFailed";
        player!.SetServerIntent(server!.ServerId, PlayerModel.ServerIntentType.CreateGame);
        return $"200\nEndpoint:{server.Endpoint}";
    }
    public string JoinGame(string authToken, string joinCode)
    {
        if (!TurrisUtils.AuthTokenValid(authToken))
            return "400\nAuthTokenInvalid";
        if (!TurrisUtils.JoinCodeValid(joinCode))
            return "400\nJoinCodeInvalid";
        bool joinedGame = false;
        PlayerModel? player = null;
        ServerModel? server = null;
        GameModel? game = null;
        if (!services.players.PlayerValid(authToken, validatedPlayer =>
        {
            player = validatedPlayer;
            joinedGame = services.servers.TryJoinGame(validatedPlayer, joinCode, out game);
        })) return "400\nAuthTokenInvalid";
        if (!joinedGame)
            return "400\nJoinGameFailed";
        player!.SetServerIntent(server!.ServerId, PlayerModel.ServerIntentType.JoinGame);
        return $"200\nEndpoint:{server.Endpoint}";
    }
}