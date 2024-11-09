using System.Reflection;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Berrevoets.Play.Economy.Common.MassTransit;

public static class Extensions
{
    public static IHostApplicationBuilder AddMassTransitWithRabbitMq(this IHostApplicationBuilder builder,
                                                                     string                       prefix)
    {
        builder.Services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(prefix, false));
            });
        });

        builder.Services.AddMassTransitHostedService();

        return builder;
    }
}