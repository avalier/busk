using System;
using CloudNative.CloudEvents;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Avalier.Busk
{
    public class CloudEventTests
    {
        private readonly ITestOutputHelper _output;

        public CloudEventTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanCreateCloudEvent()
        {
            // Arrange //
            var source = new Uri("/Avalier.Busk");
            var type = "Avalier.Busk.CloudEventTests.CanCreateCloudEvent";
            var id = Guid.NewGuid().ToString();
            var data = "Hello World";

            var cloudEvent = new CloudEvent(type, source) {
                Id = id,
                Data = data
            };

            var json = JsonConvert.SerializeObject(cloudEvent);

            _output.WriteLine(json);

            // Act //

            // Assert //
            true.ShouldBe(true);
        }
    }
}
