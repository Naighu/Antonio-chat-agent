using System;
using Antonio.Core.Abstractions;
using Antonio.Infrastructure.Llm;
using Antonio.Infrastructure.Llm.Claude;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Antonio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<ClaudeLlmOptions>()
            .Bind(config.GetSection(ClaudeLlmOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();                 // no key → app refuses to start

        services.AddHttpClient<ILlmClient, ClaudeLlmClient>((sp, http) =>
        {
            var o = sp.GetRequiredService<IOptions<ClaudeLlmOptions>>().Value;
            http.BaseAddress = new Uri(o.BaseUrl);
            http.DefaultRequestHeaders.Add("x-api-key", o.ApiKey);
            http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            http.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }
}
