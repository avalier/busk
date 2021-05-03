using System;
using System.Threading.Tasks;

namespace Avalier.Busk
{
    public interface IClient
    {
        string Endpoint { get; }

        Task PublishAsync<T>(T message) where T : class;
    }
    
}
