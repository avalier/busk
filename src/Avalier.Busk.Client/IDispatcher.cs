using System.Threading.Tasks;
using CloudNative.CloudEvents;

namespace Avalier.Busk
{
    public interface IDispatcher
    {
        Task ExecuteAsync(CloudEvent @event);
    }
}