using System.Threading.Tasks;

namespace Noobot.Domain.MessagingPipeline
{
    public interface IMiddleware
    {
        Task<Response> Invoke(IncomingMessage message);
    }
}