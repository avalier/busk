using System.Threading.Tasks;

namespace Avalier.Busk
{
    public interface IHandler
    {
    }
    
    public interface IHandler<TEvent> : IHandler
        where TEvent : class
    {
        Task ExecuteAsync(TEvent message);
    }
}