﻿namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddClientSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IStateService, StateService>();
        services.AddScoped<IPubSubService, PubSubService>();

        services.AddTransient<AppHttpClientHandler>();

        services.AddScoped<NavManuService>();

        services.AddBitBlazorUIServices();

        return services;
    }
}
