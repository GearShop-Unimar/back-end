using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GearShop.Models;
using GearShop.Repositories;
using GearShop.Services.CartService;
using NSubstitute;

namespace GearShop.Tests.Services.Carts;

public class CartServiceTests
{
    private readonly ICartRepository _cartRepository = Substitute.For<ICartRepository>();
    private readonly CartService _sut;

    public CartServiceTests()
    {
        _sut = new CartService(_cartRepository);
    }

    [Fact]
    public async Task GetCartAsync_ShouldCreateCart_WhenUserHasNoCart()
    {
        const int userId = 12;
        _cartRepository.GetCartByUserIdAsync(userId).Returns((Cart?)null);

        var result = await _sut.GetCartAsync(userId);

        result.UserId.Should().Be(userId);
        result.Items.Should().BeEmpty();
        await _cartRepository.Received(1).CreateCartAsync(Arg.Is<Cart>(cart => cart.UserId == userId));
    }

    [Fact]
    public async Task AddItemToCartAsync_ShouldIncrementQuantity_WhenItemAlreadyExists()
    {
        const int userId = 4;
        const int productId = 7;
        var cartItem = new CartItem
        {
            Id = 11,
            CartId = 2,
            ProductId = productId,
            Quantity = 1
        };
        var cart = new Cart
        {
            Id = 2,
            UserId = userId,
            Items = new List<CartItem> { cartItem }
        };

        _cartRepository.GetCartByUserIdAsync(userId).Returns(cart);

        await _sut.AddItemToCartAsync(userId, productId, 3);

        await _cartRepository.Received(1)
            .AddItemAsync(Arg.Is<CartItem>(item => item.Id == cartItem.Id && item.Quantity == 4));
    }

    [Fact]
    public async Task AddItemToCartAsync_ShouldAddNewItem_WhenProductIsNotInCart()
    {
        const int userId = 8;
        const int productId = 14;
        var cart = new Cart
        {
            Id = 5,
            UserId = userId,
            Items = new List<CartItem>()
        };

        _cartRepository.GetCartByUserIdAsync(userId).Returns(cart);

        await _sut.AddItemToCartAsync(userId, productId, 2);

        await _cartRepository.Received(1)
            .AddItemAsync(Arg.Is<CartItem>(item =>
                item.CartId == cart.Id &&
                item.ProductId == productId &&
                item.Quantity == 2));
    }

    [Fact]
    public async Task RemoveItemFromCartAsync_ShouldDelegateToRepository()
    {
        await _sut.RemoveItemFromCartAsync(99);

        await _cartRepository.Received(1).RemoveItemAsync(99);
    }
}

