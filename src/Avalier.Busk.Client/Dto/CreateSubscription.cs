using System.Collections.Generic;

namespace Avalier.Busk.Dto
{
    public class CreateSubscription
    {
        public string Endpoint { get; set; } = "";

        public List<string> Topics { get; set; } = new List<string>();
    }
}