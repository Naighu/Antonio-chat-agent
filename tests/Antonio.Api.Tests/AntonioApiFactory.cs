using Antonio.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Antonio.Api.Tests.Fakes;
namespace Antonio.Api.Tests;

public sealed class AntonioApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Antonio:Llm:Claude:ApiKey", "test-key");

        builder.ConfigureTestServices(services =>         
        {
            services.RemoveAll<ILlmClient>();
            services.AddSingleton<ILlmClient, EchoLlmClient>();
        });
    }
}
