using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGameWebAPI.Domain.Interfaces.Repositories;
using PaymentsProcessorService.Application.Services;
using PaymentsProcessorService.Infra.Integration.PaymentService;

namespace PaymentsProcessorService.Api.IoC
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddRepositories();
            services.AddServices();
            services.AddHttpClient();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPaymentStatusChangedEventRepository, PaymentStatusChangedEventRepository>();
            services.AddScoped<IPaymentCreatedEventRepository, PaymentCreatedEventRepository>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<PaymentService>();
            services.AddScoped<PaymentServiceIntegration>();
            services.AddScoped<SendEmailStatusFunction>();
            return services;
        }
    }

}
