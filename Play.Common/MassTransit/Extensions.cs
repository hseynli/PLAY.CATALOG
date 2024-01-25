using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;
using System;
using System.Reflection;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((ctx, cfg) =>
                {
                    IConfiguration configuration = ctx.GetService<IConfiguration>();
                    ServiceSettings serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    RabbitMQSettings rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    cfg.Host(rabbitMQSettings.Host);
                    ctx.ConfigureEndpoints(cfg, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    cfg.UseMessageRetry(retryConfugirator =>
                    {
                        retryConfugirator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}
