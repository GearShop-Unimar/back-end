using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using GearShop.Dtos.Payment;
using GearShop.Models;
using GearShop.Models.Enums;
using GearShop.Repositories;
using GearShop.Services.Premium;
using NSubstitute;

namespace GearShop.Tests.Services.Premium;

public class PremiumAccountServiceTests
{
    private readonly IPremiumAccountRepository _premiumAccountRepository = Substitute.For<IPremiumAccountRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPaymentRepository _paymentRepository = Substitute.For<IPaymentRepository>();
    private readonly PremiumAccountService _sut;

    public PremiumAccountServiceTests()
    {
        _sut = new PremiumAccountService(_premiumAccountRepository, _userRepository, _paymentRepository);
    }

    [Fact]
    public async Task ActivatePremiumAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        _userRepository.GetByIdAsync(5).Returns((User?)null);

        await FluentActions
            .Invoking(() => _sut.ActivatePremiumAsync(5, 30, 49.9m))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task ActivatePremiumAsync_ShouldCreateAccountAndPayment_WhenNoExistingAccount()
    {
        const int userId = 9;
        _userRepository.GetByIdAsync(userId).Returns(new User { Id = userId });
        _premiumAccountRepository.GetByUserIdAsync(userId).Returns((PremiumAccount?)null);

        PremiumAccount? createdAccount = null;
        _premiumAccountRepository.CreateAsync(Arg.Do<PremiumAccount>(account =>
        {
            createdAccount = account;
            account.Id = 321;
        })).Returns(call => call.Arg<PremiumAccount>());

        Payment? storedPayment = null;
        _paymentRepository.CreateAsync(Arg.Do<Payment>(payment => storedPayment = payment))
            .Returns(call => call.Arg<Payment>());

        var result = await _sut.ActivatePremiumAsync(userId, 15, 99.9m);

        createdAccount.Should().NotBeNull();
        createdAccount!.UserId.Should().Be(userId);
        createdAccount.Status.Should().Be(SubscriptionStatus.Active);
        createdAccount.StartDate.Should().BeCloseTo(DateTime.Now, 1.Seconds());
        createdAccount.EndDate.Should().BeAfter(createdAccount.StartDate);

        storedPayment.Should().NotBeNull();
        storedPayment!.Amount.Should().Be(99.9m);
        storedPayment.PremiumAccountId.Should().Be(321);
        storedPayment.IsRecurring.Should().BeTrue();

        result.Id.Should().Be(321);
    }

    [Fact]
    public async Task IsUserPremiumAsync_ShouldReturnFalse_AndExpireAccount_WhenEndDatePassed()
    {
        var account = new PremiumAccount
        {
            Id = 10,
            UserId = 2,
            Status = SubscriptionStatus.Active,
            EndDate = DateTime.Now.AddDays(-1)
        };

        _premiumAccountRepository.GetByUserIdAsync(account.UserId).Returns(account);

        var result = await _sut.IsUserPremiumAsync(account.UserId);

        result.Should().BeFalse();
        await _premiumAccountRepository.Received(1)
            .UpdateAsync(Arg.Is<PremiumAccount>(a => a.Status == SubscriptionStatus.Expired));
    }

    [Fact]
    public async Task GetPaymentHistoryAsync_ShouldReturnMappedDtos()
    {
        var account = new PremiumAccount { Id = 50, UserId = 7 };
        var payments = new List<Payment>
        {
            new()
            {
                Id = 1,
                Amount = 10,
                PaymentType = PaymentType.Pix,
                Status = PaymentStatus.Approved,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow,
                IsRecurring = false
            }
        };

        _premiumAccountRepository.GetByUserIdAsync(account.UserId).Returns(account);
        _paymentRepository.GetByPremiumAccountIdAsync(account.Id).Returns(payments);

        var result = await _sut.GetPaymentHistoryAsync(account.UserId);

        result.Should().ContainSingle(p =>
            p.Id == 1 &&
            p.Amount == 10 &&
            p.PaymentType == PaymentType.Pix);
    }
}

