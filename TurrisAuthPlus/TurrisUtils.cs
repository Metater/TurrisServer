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
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$") &&
            username.Length >= 1 &&
            username.Length <= 16;
    }
}