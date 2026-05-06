using FluentAssertions;
using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Listings.Queries.GetListingById;
using Heven.Api.Domain.Entities;
using Heven.Api.Domain.Enums;
using NUnit.Framework;

namespace Heven.Api.Application.FunctionalTests.Listings.Queries;

using static Testing;

public class GetListingByIdTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnListingWhenIdIsValid()
    {
        // Arrange
        // Bắt buộc lấy User ID cho những bảng yêu cầu xác thực HostId
        var userId = await RunAsDefaultUserAsync();

        // Cần tạo Category và Location trước để tránh lỗi FK Constraint giống như lúc nãy
        var category = new Category { Name = "Resort", SortOrder = 1 };
        await AddAsync(category);

        var location = new Location { Address = "abc", City = "HCM", State = "Q1", Country = "VN" };
        await AddAsync(location);

        // Tạo Data mẫu
        var listing = new Listing
        {
            HostId = userId,
            CategoryId = category.Id,
            LocationId = location.Id,
            Title = "Resort Test",
            PricePerNight = 1000000,
            MaxGuests = 4,
            Bedrooms = 2,
            Status = ListingStatus.Active
        };
        await AddAsync(listing); // Hàm AddAsync mồi sẵn của BaseTestFixture

        var query = new GetListingByIdQuery { Id = listing.Id };

        // Act
        // Gửi qua MediatR y hệt như những gì API Route của bạn đang làm
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(listing.Id);
        result.Title.Should().Be("Resort Test");
        result.PricePerNight.Should().Be(1000000);
        result.Bedrooms.Should().Be(2);
    }

    [Test]
    public async Task ShouldRequireValidListingIdAndThrowNotFoundException()
    {
        // Arrange
        var query = new GetListingByIdQuery { Id = 999999 }; // Một ID không tồn tại

        // Act & Assert
        // Phải ném ra lỗi NotFoundException
        await FluentActions.Invoking(() => SendAsync(query))
            .Should().ThrowAsync<NotFoundException>();
    }
}
