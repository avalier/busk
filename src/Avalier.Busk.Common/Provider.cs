using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Routing.Matching;
//using Microsoft.Extensions.Logging;

namespace Avalier.Busk
{
    public class Provider : IProvider
    {
        private ILogger<Provider> _logger;
        private IStorageProvider _storageProvider;
        private IQueueingProvider _queueingProvider;
        
        public Provider(
            ILogger<Provider> logger,
            IStorageProvider storageProvider,
            IQueueingProvider queueingProvider
        )
        {
            _logger = logger;
            _storageProvider = storageProvider;
            _queueingProvider = queueingProvider;
        }

        public async Task PublishAsync(CloudEvent cloudEvent)
        {
            _logger.LogInformation("Publishing event {Type} with id {Id} from {Source}", cloudEvent.Type, cloudEvent.Id, cloudEvent.Source);
            
            var httpClientHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            var httpClient = new HttpClient(httpClientHandler);
            
            var content = new CloudEventContent(
                cloudEvent,
                ContentMode.Structured,
                new JsonEventFormatter()
            );
            
            foreach (var endpoint in await _storageProvider.GetSubscriptionsAsync(cloudEvent.Type))
            {
                var response = await httpClient.PostAsync(endpoint, content);
            }
        }

        public async Task SubscribeAsync(string type, string endpoint)
        {
            await _storageProvider.CreateSubscriptionAsync(type, endpoint);
        }

        public async Task UnsubscribeAsync(string type, string endpoint)
        {
            await _storageProvider.DeleteSubscriptionAsync(type, endpoint);
        }

        public IDisposable Start() => new Disposable();
    }
}