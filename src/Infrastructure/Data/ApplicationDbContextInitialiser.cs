using System.Runtime.InteropServices;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Entities;
using Heven.Api.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // 1. Default roles
        var roles = new[] { Roles.Administrator, Roles.Host, Roles.Guest };

        foreach (var roleName in roles)
        {
            var role = new IdentityRole(roleName);

            if (_roleManager.Roles.All(r => r.Name != role.Name))
            {
                await _roleManager.CreateAsync(role);
            }
        }

        // 2. Default users
        // Admin
        var administrator = await _userManager.FindByEmailAsync("administrator@localhost");
        if (administrator == null)
        {
            administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRolesAsync(administrator, new[] { Roles.Administrator });
        }

        // Host
        var hostUser = await _userManager.FindByEmailAsync("host@localhost");
        if (hostUser == null)
        {
            hostUser = new ApplicationUser { UserName = "host@localhost", Email = "host@localhost" };
            await _userManager.CreateAsync(hostUser, "Host123456!");
            await _userManager.AddToRolesAsync(hostUser, new[] { Roles.Host });
        }

        // Guest
        var guestUser = await _userManager.FindByEmailAsync("guest@localhost");
        if (guestUser == null)
        {
            guestUser = new ApplicationUser { UserName = "guest@localhost", Email = "guest@localhost" };
            await _userManager.CreateAsync(guestUser, "Guest123456!");
            await _userManager.AddToRolesAsync(guestUser, new[] { Roles.Guest });
        }

        // 3. Default data (Seed, if necessary)
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
                {
                    new TodoItem { Title = "Make a todo list 📃" },
                    new TodoItem { Title = "Check off the first item ✅" },
                    new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                    new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
                }
            });
            await _context.SaveChangesAsync();
        }

        // Tạo Danh mục
        if (!_context.Categories.Any())
        {
            _context.Categories.AddRange(
                new Category { Name = "Nhà bên bờ biển", Icon = "beach", SortOrder = 1 },
                new Category { Name = "Biệt thự hồ bơi", Icon = "pool", SortOrder = 2 },
                new Category { Name = "Khu cắm trại", Icon = "camping", SortOrder = 3 }
            );
            await _context.SaveChangesAsync();
        }

        // Tạo Vị trí
        if (!_context.Locations.Any())
        {
            _context.Locations.AddRange(
                new Location { Address = "Bãi Dài", City = "Cam Ranh", State = "Khánh Hòa", Country = "Việt Nam", Latitude = 12.062060, Longitude = 109.213210 },
                new Location { Address = "Hồ Tuyền Lâm", City = "Đà Lạt", State = "Lâm Đồng", Country = "Việt Nam", Latitude = 11.905625, Longitude = 108.432658 }
            );
            await _context.SaveChangesAsync();
        }

        // Tạo Các Tiện ích (Amenities)
        if (!_context.Amenities.Any())
        {
            _context.Amenities.AddRange(
                new Amenity { Name = "Wifi tốc độ cao", Icon = "wifi", Category = "Internet" },
                new Amenity { Name = "Hồ bơi vô cực", Icon = "pool", Category = "Giải trí" },
                new Amenity { Name = "Máy lạnh", Icon = "ac", Category = "Nội thất" }
            );
            await _context.SaveChangesAsync();
        }

        // Tạo Bài Đăng (Listing) cùng với Images và ListingAmenities
        if (!_context.Listings.Any())
        {
            var category = await _context.Categories.FirstOrDefaultAsync();
            var location = await _context.Locations.FirstOrDefaultAsync();
            var amenities = await _context.Amenities.Take(2).ToListAsync();

            if (category != null && location != null && hostUser != null)
            {
                var listing = new Listing
                {
                    HostId = hostUser.Id,
                    CategoryId = category.Id,
                    LocationId = location.Id,
                    Title = "Biệt thự sát biển nhìn ra biển lộng gió",
                    Description = "Trải nghiệm kỳ nghỉ tuyệt vời cùng gia đình.",
                    PricePerNight = 1200000,
                    CleaningFee = 250000,
                    MaxGuests = 6,
                    Bedrooms = 3,
                    Beds = 4,
                    Bathrooms = 2,
                    PropertyType = "Villa",
                    InstantBook = true,
                    Status = Heven.Api.Domain.Enums.ListingStatus.Active,
                    RatingAverage = 5.0,
                    ReviewCount = 1,
                    Images = new List<ListingImage>
                    {
                        new ListingImage { Url = "https://example.com/image1.jpg", IsPrimary = true },
                        new ListingImage { Url = "https://example.com/image2.jpg", IsPrimary = false }
                    }
                };

                // Thêm tiện ích vào (bảng trung gian N-N)
                foreach (var amenity in amenities)
                {
                    listing.Amenities.Add(new ListingAmenity { AmenityId = amenity.Id });
                }

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();

                // Lấy Listing vừa tạo để nối FK cho các bảng dưới
                var savedListing = await _context.Listings.FirstAsync();

                // Thêm Booking (Nối với Listing và Guest)
                var booking = new Booking
                {
                    ListingId = savedListing.Id,
                    GuestId = guestUser!.Id,
                    CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                    CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                    GuestCount = 4,
                    TotalPrice = 2650000, // (1.2m * 2) + 250k fee
                    Status = Heven.Api.Domain.Enums.BookingStatus.Completed, // Giả lập đã hoành thành để review
                    SpecialRequests = "Vui lòng chuẩn bị lò nướng BBQ."
                };
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Thêm Payment (Nối với Booking)
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Amount = 2650000,
                    Method = "Credit Card",
                    Status = Heven.Api.Domain.Enums.PaymentStatus.Completed,
                    TransactionId = "TXN_123456789",
                    PaidAt = DateTimeOffset.UtcNow
                };
                _context.Payments.Add(payment);

                // Thêm Review (Nối với Booking, Listing, Guest, Host)
                var review = new Review
                {
                    BookingId = booking.Id,
                    ListingId = savedListing.Id,
                    AuthorId = guestUser.Id,
                    TargetId = hostUser.Id,
                    Type = Heven.Api.Domain.Enums.ReviewType.GuestToHost,
                    OverallRating = 5,
                    Comment = "Một trải nghiệm quá tuyệt vời, chủ nhà thân thiện!"
                };
                _context.Reviews.Add(review);

                // Thêm Conversation (Nối với Listing, Host, Guest)
                var conversation = new Conversation
                {
                    ListingId = savedListing.Id,
                    GuestId = guestUser.Id,
                    HostId = hostUser.Id,
                    LastMessageAt = DateTimeOffset.UtcNow,
                    Messages = new List<Message>
                    {
                        new Message
                        {
                            SenderId = guestUser.Id,
                            Content = "Chào anh, nhà mình còn phòng cuối tuần này không ạ?",
                            SentAt = DateTimeOffset.UtcNow.AddHours(-1)
                        },
                        new Message
                        {
                            SenderId = hostUser.Id,
                            Content = "Chào bạn, nhà mình vẫn còn trống nhé!",
                            SentAt = DateTimeOffset.UtcNow
                        }
                    }
                };
                _context.Conversations.Add(conversation);

                // Thêm Notification
                _context.Notifications.AddRange(
                    new Notification
                    {
                        UserId = hostUser.Id,
                        Type = Heven.Api.Domain.Enums.NotificationType.Booking,
                        Title = "Có đặt phòng mới",
                        Body = "Guest vừa đặt phòng của bạn vào cuối tuần này.",
                        IsRead = false
                    },
                    new Notification
                    {
                        UserId = guestUser.Id,
                        Type = Heven.Api.Domain.Enums.NotificationType.System,
                        Title = "Chào mừng đến với Heven",
                        Body = "Tài khoản của bạn đã kích hoạt thành công.",
                        IsRead = true,
                        ReadAt = DateTimeOffset.UtcNow
                    }
                );

                await _context.SaveChangesAsync();
            }
        }
    }
}
