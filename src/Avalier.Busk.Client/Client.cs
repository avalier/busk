using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avalier.Busk
{
    public class Client : IClient
    {
        private ILogger<Client> _logger;
        private System.Net.Http.HttpClient _httpClient;
        
        public Uri Source { get; protected set; }
        
        public string Endpoint { get; protected set; }
        
        protected Client(HttpClient httpClient, ILogger<Client> logger, string source, string endpoint)
        {
            _httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            _logger = logger.ThrowIfNull(nameof(logger));
            this.Source = new Uri(source);
            this.Endpoint = endpoint.ThrowIfNullOrEmpty(nameof(endpoint));
        }
        
        public async Task PublishAsync<T>(T message)
            where T : class
        {
            //var json = JsonConvert.SerializeObject(message);

            var topic = message.GetType().FullName;
            
            var cloudEvent = new CloudEvent(topic, this.Source)
            {
                DataContentType = new ContentType(MediaTypeNames.Application.Json),
                Data = message
            };
            
            var content = new CloudEventContent(
                cloudEvent,
                ContentMode.Structured,
                new JsonEventFormatter()
            );
            
            var response = await _httpClient.PostAsync(this.Endpoint, content);
        }

        public static IClient Create(ILogger<Client> logger, string source, string endpoint)
        {
            // Validate //
            logger.ThrowIfNull(nameof(logger));
            source.ThrowIfNullOrEmpty(nameof(source));
            endpoint.ThrowIfNullOrEmpty(nameof(endpoint));
            
            var httpClientHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            var httpClient = new HttpClient(httpClientHandler);
            var buskClient = new Client(httpClient, logger, source, endpoint);
            
            return buskClient;
        }
    }
}
