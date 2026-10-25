using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Application.Common.Interfaces;
using Patient.Infrastructure.Persistence;
using Patient.Infrastructure.Services;
using System.Reflection;
using MediatR;
using Patient.Application.Common.Behaviors;

namespace Patient.Infrastructure;

// Dependency Injection extension for Patient.Infrastructure layer
public static class DependencyInjection
{
    public static IServiceCollection AddPatientInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure SqlServer DbContext with the correct connection string name
        services.AddDbContext<PatientDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PatientDbContext"),
                b => b.MigrationsAssembly(typeof(PatientDbContext).Assembly.FullName)));

        // Register IPatientDbContext interface
        services.AddScoped<IPatientDbContext>(provider =>
            provider.GetRequiredService<PatientDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register all validators from Patient.Application assembly automatically
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register ValidationBehavior into MediatR pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}