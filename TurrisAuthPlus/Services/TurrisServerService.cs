﻿namespace TurrisAuthPlus.Services;

public class TurrisServerService
{
    private TurrisServices services;

    public TurrisServerService(TurrisServices services)
    {
        this.services = services;
    }

    public string DeleteAccount(string username)
    {
        services.accounts.DeleteAccount(username);
        return "200";
    }
}