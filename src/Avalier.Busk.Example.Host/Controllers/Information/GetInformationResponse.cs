using System.Collections.Generic;

namespace Avalier.Busk.Example.Host.Controllers.Information
{
    public class GetInformationResponse
    {
        public string Version { get; internal set; }
        
        public string Name { get; internal set; }
        
        public string Hostname { get; internal set; }
        
        public List<string> Addresses { get; internal set; }
    }
}
