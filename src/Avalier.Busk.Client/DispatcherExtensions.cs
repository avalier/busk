using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Avalier.Busk
{
    public static class DispatcherExtensions
    {
        public static DispatcherContext AddHandler(this DispatcherContext context, string topic, Type handlerType)
        {
            if (!context.HandlersByTopic.ContainsKey(topic))
            {
                context.HandlersByTopic.Add(topic, new List<Type>());
            }
            if (!context.HandlersByTopic[topic].Contains(handlerType))
            {
                context.HandlersByTopic[topic].Add(handlerType);
            }
            return context;
        }

        public static Type GetHandlerInterface(this Type handlerType)
        {
            var i = handlerType.GetTypeInfo().ImplementedInterfaces
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IHandler<>))
                .FirstOrDefault();
            
            if (null == i)
            {
                throw new NullReferenceException("Unable to retrieve interface IHandler<TEvent> from the specified type");
            }
            
            return i;
        }
    }
}