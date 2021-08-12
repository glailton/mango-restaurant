﻿using Bogus;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;

namespace Mango.Services.ProductAPI.Tests.Fixture
{
    public class ProductFixture
    {
        public Faker<Product> ProductBuilder()
        {
            return new Faker<Product>()
                .RuleFor(o => o.ProductId, f => f.Random.Int(min: 1))
                .RuleFor(o => o.Name, f => f.Commerce.ProductName())
                .RuleFor(o => o.Price, f => f.Random.Double(min: 1, max: 1000))
                .RuleFor(o => o.Description, f => f.Commerce.ProductDescription())
                .RuleFor(o => o.CategoryName, f => f.Name.FirstName())
                .RuleFor(o => o.ImageUrl, f => f.Image.PicsumUrl());
        }
    }
}