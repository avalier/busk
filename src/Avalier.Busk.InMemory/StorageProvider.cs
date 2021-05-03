using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing.Matching;
//using Microsoft.Extensions.Logging;

namespace Avalier.Busk.InMemory
{
    /// <summary>
    /// In memory implementation of the storage/persistence provider (thread safe). 
    /// </summary>
    public class StorageProvider : IStorageProvider
    {
        private readonly IDictionary<string, List<string>> _topics = new ConcurrentDictionary<string, List<string>>();

        private object _lock = new object();
        
        public async Task CreateSubscriptionAsync(string type, string endpoint)
        {
            await Task.Delay(0);
            lock (_lock)
            {
                var endpoints = _topics.ContainsKey(type)
                    ? _topics[type].ToList()
                    : new List<string>();

                if (!endpoints.Contains(endpoint))
                {
                    endpoints.Add(endpoint);
                }

                _topics[type] = endpoints;
            }
        }
        
        public async Task DeleteSubscriptionAsync(string type, string endpoint)
        {
            await Task.Delay(0);

            lock (_lock)
            {
                if (!_topics.ContainsKey(type)) return;

                var endpoints = _topics[type].ToList();

                if (endpoints.Contains(endpoint))
                {
                    endpoints.Remove(endpoint);
                }

                if (endpoints.Count == 0)
                {
                    _topics.Remove(type);
                }
            }
        }

        public async Task<IList<string>> GetSubscriptionsAsync(string type)
        {
            await Task.Delay(0);
            return _topics.ContainsKey(type)
                ? _topics[type]
                : new List<string>().AsReadOnly();
        }
    }
}