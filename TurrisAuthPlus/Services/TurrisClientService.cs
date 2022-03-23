namespace TurrisAuthPlus.Services;

public class TurrisClientService
{
    private TurrisServices services;

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
        if (!services.players.PlayerValid(authToken, out PlayerModel player))
            return "400\nAuthTokenInvalid";

        return $"{authToken}";
    }
    public string JoinGame(string authToken)
    {
        return $"{authToken}";
    }
}