using System.Collections.Generic;
using System.Threading.Tasks;

namespace Avalier.Busk
{
    public interface IStorageProvider
    {
        Task CreateSubscriptionAsync(string type, string endpoint);

        Task DeleteSubscriptionAsync(string type, string endpoint);
        
        Task<IList<string>> GetSubscriptionsAsync(string type);
    }
}