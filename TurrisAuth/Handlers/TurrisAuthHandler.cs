namespace TurrisAuth.Handlers;

public abstract class TurrisAuthHandler
{
    protected WebApplication app;
    protected TurrisAuthData data;
    protected TurrisValidation validation;

    public TurrisAuthHandler(WebApplication app, TurrisAuthData data, TurrisValidation validation)
    {
        this.app = app;
        this.data = data;
        this.validation = validation;
    }
}