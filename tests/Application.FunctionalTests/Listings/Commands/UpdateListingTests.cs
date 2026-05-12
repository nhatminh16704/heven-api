using FluentAssertions;
using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Listings.Commands.CreateListing;
using Heven.Api.Application.Listings.Commands.UpdateListing;
using Heven.Api.Domain.Entities;
using NUnit.Framework;

namespace Heven.Api.Application.FunctionalTests.Listings.Commands;

using static Testing;

public class UpdateListingTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidListingId()
    {
        await RunAsAdministratorAsync();
        
        var command = new UpdateListingCommand
        {
            Id = 9999,
            CategoryId = 1,
            Title = "New Title",
            PricePerNight = 1000,
            MaxGuests = 2,
            Beds = 1,
            Bathrooms = 1,
            Location = new UpdateListingLocationDto { Address = "123 Street", CityId = 1 }
        };

        await FluentActions.Invoking(() => SendAsync(command))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateListing()
    {
        var userId = await RunAsAdministratorAsync();

        var category = new Category { Name = "Resort", Icon = "home", SortOrder = 1 };
        await AddAsync(category);

        var country = new Country { Name = "Vietnam", Code = "VN" };
        var state = new State { Name = "Khanh Hoa", Country = country };
        var city = new City { Name = "Nha Trang", State = state };
        await AddAsync(city);

        var amenity1 = new Amenity { Name = "Wifi", Icon = "wifi", Category = "Internet" };
        var amenity2 = new Amenity { Name = "Pool", Icon = "pool", Category = "Entertainment" };
        await AddAsync(amenity1);
        await AddAsync(amenity2);

        var command = new CreateListingCommand
        {
            CategoryId = category.Id,
            Title = "Biệt thự sát biển",
            PricePerNight = 2500000,
            MaxGuests = 8,
            Beds = 5,
            Bathrooms = 3,
            Location = new CreateListingLocationDto
            {
                Address = "123 Đường Bờ Biển",
                CityId = city.Id,
            },
            AmenityIds = new[] { amenity1.Id }
        };

        var listingId = await SendAsync(command);

        var updateCommand = new UpdateListingCommand
        {
            Id = listingId,
            CategoryId = category.Id,
            Title = "Biệt thự sát biển (Đã cập nhật)",
            Description = "Mới được sơn lại",
            PricePerNight = 3000000,
            CleaningFee = 400000,
            MaxGuests = 10,
            Bedrooms = 5,
            Beds = 6,
            Bathrooms = 4,
            PropertyType = "Villa",
            InstantBook = false,
            Location = new UpdateListingLocationDto
            {
                Address = "456 Đường Mới",
                CityId = city.Id,
                Latitude = 10.0,
                Longitude = 20.0
            },
            AmenityIds = new[] { amenity2.Id } // Xóa Wifi, thêm Pool
        };

        await SendAsync(updateCommand);

        var listing = await FindAsync<Listing>(listingId);

        listing.Should().NotBeNull();
        listing!.Title.Should().Be(updateCommand.Title);
        listing.Description.Should().Be(updateCommand.Description);
        listing.PricePerNight.Should().Be(updateCommand.PricePerNight);
        listing.CleaningFee.Should().Be(updateCommand.CleaningFee);
        listing.MaxGuests.Should().Be(updateCommand.MaxGuests);
        listing.Bedrooms.Should().Be(updateCommand.Bedrooms);
        listing.Beds.Should().Be(updateCommand.Beds);
        listing.Bathrooms.Should().Be(updateCommand.Bathrooms);
        listing.PropertyType.Should().Be(updateCommand.PropertyType);
        listing.InstantBook.Should().BeFalse();
    }
}
