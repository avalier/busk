using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Avalier.Busk.Controllers.Subscription.Dto {

    public class SubscriptionLine
    {
        public string Source { get; set; }

        public string Type { get; set; }

        public string Endpoint { get; set; }
    }
}