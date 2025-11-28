using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GearShop.Dtos.Product;
using GearShop.Models;
using GearShop.Repositories;
using GearShop.Services.Product;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace GearShop.Tests.Services.Products;

public class ProductServiceTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_productRepository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldMapProductsToDtos()
    {
        var imageBytes = new byte[] { 1, 2, 3 };
        var products = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Mouse Gamer",
                Description = "RGB",
                Price = 199.90m,
                StockQuantity = 20,
                Category = "Peripherals",
                SellerId = 5,
                ImageData = imageBytes,
                ImageMimeType = "image/png",
                Reviews = new List<ProductReview>
                {
                    new() { Rating = 4 },
                    new() { Rating = 2 }
                }
            },
            new()
            {
                Id = 2,
                Name = "Monitor 4K",
                Description = "Painel IPS",
                Price = 2399.90m,
                StockQuantity = 3,
                Category = "Displays",
                SellerId = 6
            }
        };

        _productRepository.GetAllAsync().Returns(products);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].MainImageUrl.Should().Be($"data:image/png;base64,{Convert.ToBase64String(imageBytes)}");
        result[0].AverageRating.Should().Be(3f); // (4 + 2) / 2
        result[0].ReviewCount.Should().Be(2);
        result[1].MainImageUrl.Should().BeEmpty();
        result[1].AverageRating.Should().Be(0);
        result[1].ReviewCount.Should().Be(0);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        _productRepository.GetByIdAsync(Arg.Any<int>()).Returns((Product?)null);

        var dto = new UpdateProductDto
        {
            Name = "Updated name",
            Description = "Updated description",
            Price = 99.90m,
            StockQuantity = 5,
            Category = "Peripherals",
            ImageFile = Substitute.For<IFormFile>()
        };

        var result = await _sut.UpdateAsync(10, dto, null, null);

        result.Should().BeNull();
        await _productRepository.DidNotReceive().UpdateAsync(Arg.Any<int>(), Arg.Any<Product>());
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistProductWithProvidedImage()
    {
        var dto = new CreateProductDto
        {
            Name = "Teclado",
            Description = "Switch azul",
            Price = 349.90m,
            StockQuantity = 15,
            Category = "Peripherals",
            ImageFile = Substitute.For<IFormFile>(),
            SellerId = 99
        };

        var sellerId = 7;
        var imageBytes = new byte[] { 5, 6, 7 };
        const string imageMimeType = "image/jpeg";

        _productRepository.CreateAsync(Arg.Any<Product>())
            .Returns(callInfo =>
            {
                var product = callInfo.Arg<Product>();
                product.Id = 123;
                return product;
            });

        var result = await _sut.CreateAsync(dto, imageBytes, imageMimeType, sellerId);

        await _productRepository.Received(1).CreateAsync(Arg.Is<Product>(p =>
            p.Name == dto.Name &&
            p.Description == dto.Description &&
            p.Price == dto.Price &&
            p.StockQuantity == dto.StockQuantity &&
            p.Category == dto.Category &&
            p.SellerId == sellerId &&
            p.ImageData == imageBytes &&
            p.ImageMimeType == imageMimeType));

        result.Id.Should().Be(123);
        result.MainImageUrl.Should().Be($"data:{imageMimeType};base64,{Convert.ToBase64String(imageBytes)}");
    }
}

