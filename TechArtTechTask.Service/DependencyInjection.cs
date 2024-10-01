using Microsoft.Extensions.DependencyInjection;
using TechArtTechTask.Service.Interfaces;
using TechArtTechTask.Service.Services;

namespace TechArtTechTask.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddHttpClient<IKufarService, KufarService>();
            return services;
        }
    }
}
