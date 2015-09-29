namespace Noobot.Domain.Slack
{
    public interface ISlackConnector
    {
        InitialConnectionStatus Connect();
    }
}