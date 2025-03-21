using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.Repositories;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;
        private readonly IFixture _fixture;
        private readonly Guid _partnerId = new("7d994823-8226-4273-b063-1a95f3cc1df8");
        private readonly Guid _partnerPromoCodeLimitId = new("e00633a5-978a-420e-a7d6-3e1dab116393");

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = _fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = _fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        private Partner CreatePartner(bool isActive, bool limitExpired)
        {
            var limit = _fixture.Build<PartnerPromoCodeLimit>()
                .With(l => l.Id, _partnerPromoCodeLimitId)
                .With(l => l.Limit, 10)
                .With(l => l.PartnerId, _partnerId)
                .With(l => l.CancelDate, limitExpired ? DateTime.Now : null)
                .Without(l => l.Partner);

            var partner = _fixture.Build<Partner>()
                .With(p => p.Id, _partnerId)
                .With(p => p.IsActive, isActive)
                .With(p => p.NumberIssuedPromoCodes, 5)
                .With(p => p.PartnerLimits, [limit.Create()]);

            return partner.Create();
        }

        private SetPartnerPromoCodeLimitRequest CreateRequest()
            => _fixture.Build<SetPartnerPromoCodeLimitRequest>().Create();

        [Fact]
        public async Task GetPartnerLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            Partner partner = null;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(_partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(_partnerId);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task GetPartnerLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partner = CreatePartner(false, false);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.CancelPartnerPromoCodeLimitAsync(partner.Id);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerNumberIssuedPromoCodesShouldBe0_NumberIssuedPromoCodesBe0(bool limitExpired)
        {
            // Arrange
            var partner = CreatePartner(true, limitExpired);
            var numberIssuedPromoCodes = partner.NumberIssuedPromoCodes;
            var setPartnerPromoCodeLimitRequest = CreateRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            if (limitExpired)
                partner.NumberIssuedPromoCodes.Should().Be(numberIssuedPromoCodes);
            else
                partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerPreviousLimitShouldBe0_PartnerPreviousLimitIs0()
        {
            // Arrange
            var partner = CreatePartner(true, false);
            var limit = partner.PartnerLimits.FirstOrDefault();
            var setPartnerPromoCodeLimitRequest = CreateRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            limit.Should().NotBeNull();
            limit.CancelDate.HasValue.Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task SetPartnerPromoCodeLimitAsync_LimitShouldBeGreaterThan0_LimitGraterThan0(int requestLimit)
        {
            // Arrange
            var partner = CreatePartner(true, false);
            var setPartnerPromoCodeLimitRequest = CreateRequest();
            setPartnerPromoCodeLimitRequest.Limit = requestLimit;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().BeEquivalentTo("Лимит должен быть больше 0"); ;
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_CheckNewLimitCreated_LimitExists()
        {
            // Arrange
            var partner = CreatePartner(true, false);
            var limit = partner.PartnerLimits.First();
            var setPartnerPromoCodeLimitRequest = CreateRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);
            _partnersRepositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);

            var newLimit = partner.PartnerLimits.FirstOrDefault(l => !l.CancelDate.HasValue);

            //Assert            
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            limit.Should().NotBeNull();
            newLimit.Should().NotBeNull();
            newLimit.Id.Should().NotBe(limit.Id);
        }
    }
}