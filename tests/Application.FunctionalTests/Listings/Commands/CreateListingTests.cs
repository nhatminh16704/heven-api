using FluentAssertions;
using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Listings.Commands.CreateListing;
using Heven.Api.Domain.Entities;
using NUnit.Framework;

namespace Heven.Api.Application.FunctionalTests.Listings.Commands;

using static Testing;

public class CreateListingTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        await RunAsAdministratorAsync();
        var command = new CreateListingCommand
        {
            Title = "",
            Location = null!
        };

        await FluentActions.Invoking(() => SendAsync(command))
            .Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateListingWithNestedEntities()
    {
        var userId = await RunAsAdministratorAsync();

        // 1. Arrange - Khởi tạo dữ liệu liên quan để tránh lỗi Foreign Key
        var category = new Category { Name = "Resort", Icon = "home", SortOrder = 1 };
        await AddAsync(category);

        var country = new Country { Name = "Vietnam", Code = "VN" };
        var state = new State { Name = "Khanh Hoa", Country = country };
        var city = new City { Name = "Nha Trang", State = state };
        await AddAsync(city); // AddAsync will also insert Country and State automatically

        var amenity1 = new Amenity { Name = "Wifi", Icon = "wifi", Category = "Internet" };
        var amenity2 = new Amenity { Name = "Pool", Icon = "pool", Category = "Entertainment" };
        await AddAsync(amenity1);
        await AddAsync(amenity2);

        var command = new CreateListingCommand
        {
            CategoryId = category.Id,
            Title = "Biệt thự sát biển cao cấp",
            Description = "View biển cực chill",
            PricePerNight = 2500000,
            CleaningFee = 300000,
            MaxGuests = 8,
            Bedrooms = 4,
            Beds = 5,
            Bathrooms = 3,
            PropertyType = "Villa",
            InstantBook = true,
            Location = new CreateListingLocationDto
            {
                Address = "123 Đường Bờ Biển",
                CityId = city.Id,
                Latitude = 12.345678,
                Longitude = 109.876543
            },
            AmenityIds = new[] { amenity1.Id, amenity2.Id },
            Images = new[]
            {
                new CreateListingImageDto { Url = "https://example.com/main.jpg", IsPrimary = true, SortOrder = 1 },
                new CreateListingImageDto { Url = "https://example.com/sub.jpg", IsPrimary = false, SortOrder = 2 }
            }
        };

        // 2. Act - Gọi API (Send)
        var id = await SendAsync(command);

        // 3. Assert - Kiểm tra Listing mới tạo trong DB
        var listing = await FindAsync<Listing>(id);

        listing.Should().NotBeNull();
        listing!.Title.Should().Be(command.Title);
        listing.PricePerNight.Should().Be(command.PricePerNight);
        listing.HostId.Should().Be(userId);
        
        // Cần đảm bảo LocationId đã được EF Core tự mapping (lớn hơn 0)
        listing.LocationId.Should().BeGreaterThan(0);
    }
}
