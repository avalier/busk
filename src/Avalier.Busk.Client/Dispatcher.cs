using System;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Newtonsoft.Json;

namespace Avalier.Busk
{
    public class Dispatcher : IDispatcher
    {
        private IServiceProvider _serviceProvider;
        private DispatcherContext _context;

        public Dispatcher(
            IServiceProvider serviceProvider, 
            DispatcherContext context
        )
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        public async Task ExecuteAsync(CloudEvent @event)
        {
            await Task.Delay(0);
            var topic = @event.Type;
            foreach (var handlerType in _context.HandlersByTopic[topic])
            {
                var i = handlerType.GetHandlerInterface();
                var messageType = i.GetGenericArguments()[0];
                var json = @event.Data.ToString();
                var methodInfo = handlerType.GetMethod("ExecuteAsync", new Type[] { messageType });
                var handler = _serviceProvider.GetService(handlerType);
                var message = JsonConvert.DeserializeObject(json, messageType);
                methodInfo.Invoke(handler, new object[] { message });
            }
        }
    }
}