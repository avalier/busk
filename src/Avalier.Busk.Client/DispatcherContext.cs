using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Avalier.Busk
{
    public class DispatcherContext
    {

        public List<Assembly> Assemblies { get; set; } = new List<Assembly>();
        public Dictionary<string, List<Type>> HandlersByTopic { get; set; } = new Dictionary<string, List<Type>>();
        
        

        internal DispatcherContext()
        {
        }
    }
}