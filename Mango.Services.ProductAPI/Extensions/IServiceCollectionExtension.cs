using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Mango.Services.ProductAPI.Extensions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddAutoMapper(GetAssembly());
            return services;
        }

        private static Assembly GetAssembly()
        {
            return typeof(IServiceCollectionExtension).Assembly;
        }
    }
}
