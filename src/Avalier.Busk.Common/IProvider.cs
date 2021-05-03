using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CloudNative.CloudEvents;

namespace Avalier.Busk
{
    public interface IProvider
    {
        Task PublishAsync(CloudEvent cloudEvent);
        
        Task SubscribeAsync(string type, string endpoint);

        Task UnsubscribeAsync(string type, string endpoint);

        IDisposable Start();
    }
}
