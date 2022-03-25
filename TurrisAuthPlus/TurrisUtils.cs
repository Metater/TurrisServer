namespace TurrisAuthPlus;

public static class TurrisUtils
{
    public static string Hash(string data)
    {
        HashAlgorithm sha = SHA256.Create();
        byte[] result = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
        return Convert.ToBase64String(result);
    }

    public static bool PasswordValid(string password)
    {
        return password.Length >= 6 && password.Length <= 18;
    }

    public static bool UsernameValid(string username)
    {
        return username.Length >= 1 && username.Length <= 16 &&
            Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
    }

    public static bool JoinCodeValid(string joinCode)
    {
        return joinCode.Length == 4;
    }

    public static bool GuidLengthValid(string gameCode)
    {
        return gameCode.Length == 36;
    }

    public static bool EndpointLengthValid(string endpoint)
    {
        return endpoint.Length >= 1 && endpoint.Length <= 128;
    }
}