using System.Text.Json;
using System.Threading.Tasks;
using Avalier.Busk.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avalier.Busk.Handlers
{
    public class JobCompletedHandler : Avalier.Busk.IHandler<Avalier.Busk.Contracts.JobCompleted>
    {
        private ILogger<JobCompletedHandler> _logger;
        
        public JobCompletedHandler(ILogger<JobCompletedHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task ExecuteAsync(JobCompleted message)
        {
            await Task.Delay(0);
            _logger.LogInformation($"Executing {nameof(JobCompletedHandler)}: {JsonConvert.SerializeObject(message)}");
        }
    }
}