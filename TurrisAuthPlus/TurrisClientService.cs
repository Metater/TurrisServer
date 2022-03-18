﻿namespace TurrisAuthPlus;

public class TurrisClientService
{
    public async Task<string> CreateAccount(string gameCode, string username, string password)
    {
        return $"{gameCode} {username} {password}";
    }
    public async Task<string> DeleteAccount(string username, string password)
    {
        return $"{username} {password}";
    }
    public async Task<string> AuthPlayer(string username, string password)
    {
        return $"{username} {password}";
    }
    public async Task<string> CreateGame(string authToken)
    {
        return $"{authToken}";
    }
    public async Task<string> JoinGame(string authToken)
    {
        return $"{authToken}";
    }
}