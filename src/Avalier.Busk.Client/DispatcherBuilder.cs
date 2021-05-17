using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;

namespace Avalier.Busk
{
    public class DispatcherBuilder
    {
        protected DispatcherContext Context { get; } = new DispatcherContext();
        
        public string Endpoint { get; internal set; } = "";
        public string Consumer { get; internal set; } = "";
        public bool IsRegisterHandlers { get; internal set; } = false;
        public bool IsCreateSubscriptions { get; internal set; } = false;

        public DispatcherBuilder ScanAssembly(Assembly assembly)
        {
            if (this.Context.Assemblies.Contains(assembly))
            {
                return this;
            }

            this.Context.Assemblies.Add(assembly);

            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass)
                .Where(t => t.GetTypeInfo().ImplementedInterfaces
                    .Where(i => i.IsGenericType)
                    .Where(i => i.GetGenericTypeDefinition() == typeof(IHandler<>))
                    .Any()
                )
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var i = handlerType.GetHandlerInterface();
                var messageType = i.GetGenericArguments()[0];
                this.Context.AddHandler(messageType.FullName, handlerType);
            }

            return this;
        }

        public DispatcherBuilder AddHandler<THandler>()
            where THandler : IHandler
        {
            var handlerType = typeof(THandler);
            var i = handlerType.GetHandlerInterface();
            var messageType = i.GetGenericArguments()[0];
            this.Context.AddHandler(messageType.FullName, handlerType);
            return this;
        }
 
        public Dispatcher Create(IServiceProvider serviceProvider)
        {
            var dispatcher = new Dispatcher(serviceProvider, this.Context);
            return dispatcher;
        }

        public List<Type> GetHandlerTypes()
        {
            return this.Context.HandlersByTopic
                .SelectMany( 
                    kvp => kvp.Value,
                    (kvp, type) => type
                )
                .ToList();
        }

        public List<string> GetTopics()
        {
            return this.Context.HandlersByTopic
                .Select(o => o.Key)
                .ToList();
        }

        public DispatcherBuilder SetEndpoint(string endpoint)
        {
            this.Endpoint = endpoint;
            return this;
        }
        
        public DispatcherBuilder SetConsumer(string endpoint)
        {
            this.Consumer = endpoint;
            return this;
        }
        
        public DispatcherBuilder RegisterHandlers(bool value)
        {
            this.IsRegisterHandlers = value;
            return this;
        }

        public DispatcherBuilder CreateSubscriptions(bool value)
        {
            this.IsCreateSubscriptions = value;
            return this;
        }
    }
}