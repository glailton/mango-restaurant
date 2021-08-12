using System;
using AutoMapper;
using Mango.Services.ProductAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Mango.Services.ProductAPI.Tests.Fixture
{
    public class DependencyInjectionFixture
    {
        public readonly IServiceProvider _ServiceProvider;

        public DependencyInjectionFixture()
        {
            var services = new ServiceCollection();

            services.AddDomainServices();
            
            IMapper mapper = MappingConfig.RegiterMaps().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            _ServiceProvider = services.BuildServiceProvider();
        }
    }
}