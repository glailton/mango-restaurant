using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Mango.Services.ProductAPI.Repository;
using Mango.Services.ProductAPI.Tests.Fixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Mango.Services.ProductAPI.Tests.Repository
{
    public class ProductRepositoryTest : IClassFixture<DependencyInjectionFixture>, IClassFixture<ProductFixture>, 
        IClassFixture<ProductDtoFixture>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ProductFixture _productFixture;
        private readonly ProductDtoFixture _productDtoFixture;

        public ProductRepositoryTest(ProductFixture productFixture, ProductDtoFixture productDtoFixture, 
            DependencyInjectionFixture dependencyInjectionFixture)
        {
            _productFixture = productFixture;
            _productDtoFixture = productDtoFixture;
            _serviceProvider = dependencyInjectionFixture._ServiceProvider;
        }

        [Fact]
        public async void GetProducts_Return_ProductList()
        {
            var dbContextMock = new Mock<ApplicationDbContext>();
            var mapper = _serviceProvider.GetService<IMapper>();

            var productFixture = _productFixture.ProductBuilder().Generate(1);
            var productDtoFixture = _productDtoFixture.ProductBuilder().Generate(1);

            var dbSetMock = GetMockDbSet<Product>(productFixture);
            // dbSetMock
            //     .Setup(s => s.FindAsync(It.IsAny<ProductDto>()))
            //     .Returns(Task.FromResult(new Product()));
            
            var dbContext = await GetDatabaseContext();
            
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<Product, ProductDto>(It.IsAny<Product>()))
                .Returns(new ProductDto()); 
            
            dbContextMock.Setup(s => s.Set<Product>()).Returns(dbSetMock.Object);

            var repository = new ProductRepository(dbContext, mapper);

            var productDto = await repository.CreateUpdateProduct(productDtoFixture.First());

            Assert.NotNull(productDto);

        }
        
        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            // if (await databaseContext.Users.CountAsync() <= 0)
            // {
            //     for (int i = 1; i <= 10; i++)
            //     {
            //         databaseContext.Pro.Add(new User()
            //         {
            //             
            //         });
            //         await databaseContext.SaveChangesAsync();
            //     }
            // }
            return databaseContext;
        }

        internal static Mock<DbSet<T>> GetMockDbSet<T>(ICollection<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.AsQueryable().Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(entities.Add);
            return mockSet;
        }
    }
}