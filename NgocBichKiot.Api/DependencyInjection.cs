using Application.Interfaces;
using Infrastructures;
using NgocBichKiot.Api.Services;
using System.Diagnostics;

namespace NgocBichKiot.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddSingleton<GlobalExceptionMiddleware>();
            //services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddHttpContextAccessor();
            //services.AddFluentValidationAutoValidation();
            //services.AddFluentValidationClientsideAdapters();
            return services;
        }
    }
}
