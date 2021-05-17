using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Avalier.Busk.Controllers.Subscription.Dto {

    public class CreateSubscription
    {
        public string Endpoint { get; set; } = "";

        public List<string> Topics { get; set; } = new List<string>();
    }
}