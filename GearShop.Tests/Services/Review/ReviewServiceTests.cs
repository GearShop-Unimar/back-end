using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using GearShop.Dtos.Product;
using GearShop.Models;
using GearShop.Repositories;
using GearShop.Services.Review;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace GearShop.Tests.Services.Review;

public class ReviewServiceTests
{
    private readonly IReviewRepository _reviewRepository = Substitute.For<IReviewRepository>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ReviewService _sut;

    public ReviewServiceTests()
    {
        _sut = new ReviewService(_reviewRepository, _productRepository);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenUserIsNotAuthenticated()
    {
        var dto = new CreateReviewDto { Rating = 5, ProductId = 10 };
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

        await FluentActions
            .Invoking(() => _sut.CreateReviewAsync(dto, anonymousUser))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenProductDoesNotExist()
    {
        var dto = new CreateReviewDto { Rating = 5, ProductId = 88 };
        var user = BuildUserPrincipal(42, "Tester");
        _productRepository.GetByIdAsync(dto.ProductId).Returns((Product?)null);

        await FluentActions
            .Invoking(() => _sut.CreateReviewAsync(dto, user))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldReturnDto_WhenCreationSucceeds()
    {
        var dto = new CreateReviewDto { Rating = 4, Comment = "Nice", ProductId = 5 };
        var user = BuildUserPrincipal(7, "Joana");
        var product = new Product { Id = dto.ProductId, Name = "Mouse" };
        _productRepository.GetByIdAsync(dto.ProductId).Returns(product);

        _reviewRepository
            .AddAsync(Arg.Do<ProductReview>(review => review.Id = 123))
            .Returns(Task.CompletedTask);

        var result = await _sut.CreateReviewAsync(dto, user);

        await _reviewRepository.Received(1).AddAsync(Arg.Is<ProductReview>(review =>
            review.ProductId == dto.ProductId &&
            review.AuthorId == 7 &&
            review.Rating == dto.Rating &&
            review.Comment == dto.Comment));

        result.Id.Should().Be(123);
        result.Rating.Should().Be(dto.Rating);
        result.Author.Name.Should().Be("Joana");
        result.Author.Id.Should().Be(7);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenUserAlreadyReviewed()
    {
        var dto = new CreateReviewDto { Rating = 3, ProductId = 9 };
        var user = BuildUserPrincipal(2, "Ana");
        _productRepository.GetByIdAsync(dto.ProductId).Returns(new Product { Id = dto.ProductId });
        _reviewRepository.AddAsync(Arg.Any<ProductReview>())
            .Returns<Task>(_ => throw new DbUpdateException());

        await FluentActions
            .Invoking(() => _sut.CreateReviewAsync(dto, user))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Já avaliaste este produto.");
    }

    [Fact]
    public async Task GetReviewsForProductAsync_ShouldMapEntitiesToDtos()
    {
        var reviews = new List<ProductReview>
        {
            new()
            {
                Id = 1,
                Rating = 5,
                Comment = "Ótimo",
                CreatedAt = DateTime.UtcNow,
                ProductId = 10,
                Author = new User { Id = 3, Name = "Rodrigo" }
            }
        };

        _reviewRepository.GetByProductIdAsync(10).Returns(reviews);

        var result = await _sut.GetReviewsForProductAsync(10);

        result.Should().ContainSingle(r =>
            r.Id == 1 &&
            r.Author.Name == "Rodrigo" &&
            r.ProductId == 10);
    }

    private static ClaimsPrincipal BuildUserPrincipal(int? id, string? name = null)
    {
        var claims = new List<Claim>();
        if (id.HasValue)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
    }
}

