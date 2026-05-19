using AutoDI.PocApp;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDI.IntegrationTests
{
    public class RegistrationIntegrationTests
    {
        [Fact]
        public void GetRequiredService_ReturnsCorrectImplementation()
        {
            var services = new ServiceCollection();
            services.AddAutoRegisteredServices();

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<IGreetingService>();

            Assert.IsType<GreetingService>(service);
        }

        [Fact]
        public void GetRequiredService_ServiceBehavesCorrectly()
        {
            var services = new ServiceCollection();
            services.AddAutoRegisteredServices();

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<IGreetingService>();

            Assert.Equal("Hello, World!", service.Greet("World"));
        }
    }
}
