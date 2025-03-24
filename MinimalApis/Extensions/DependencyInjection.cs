using Asp.Versioning.Builder;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalApis.Commands.RegisterProfile;
using MinimalApis.Commands.RequestOtp;
using MinimalApis.Commands.UpdateProfile;
using MinimalApis.Commands.VerifyOtp;
using MinimalApis.Domain;
using MinimalApis.Domain.Accounts;
using MinimalApis.Endpoints;

namespace MinimalApis.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomValidations(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterProfileCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<RequestOtpCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<VerifyOtpCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateProfileCommandValidator>();
        return services;
    }
    public static IServiceCollection RegisterEndpoints(this IServiceCollection services)
    {
        var serviceDescriptors = typeof(IEndpoint)
            .Assembly
            .DefinedTypes
            .Where(type => 
                type is { IsAbstract: false, IsInterface: false }
                && typeof(IEndpoint).IsAssignableFrom(type)
            )
            .Select(endpoint => ServiceDescriptor.Transient(typeof(IEndpoint), endpoint));
        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder routeGroupBuilder = app.MapGroup("v{apiVersion:apiVersion}")
            .WithApiVersionSet(apiVersionSet);
        
        app
            .Services
            .GetRequiredService<IEnumerable<IEndpoint>>()
            .ToList()
            .ForEach(endpoint => endpoint.MapEndpoint(routeGroupBuilder));
        return app;
    }

    public static void AddUserAccounts(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<IDbContext>();
            dbContext?
            .Accounts
            .Add(new Account { Name = "John Smith", Email = "john.smith@mailinator.com", Phone = "123456789", Status = AccountStatus.Active });
            dbContext?.SaveChanges();
        }
    }
}
