namespace Noobot.Domain.Configuration
{
    public interface IConfigReader
    {
        dynamic GetConfig();
    }
}