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

        // 2.1 Seed UserProfiles
        if (!_context.UserProfiles.Any())
        {
            _context.UserProfiles.AddRange(
                new UserProfile { UserId = administrator.Id, FirstName = "Admin", LastName = "System" },
                new UserProfile { UserId = hostUser.Id, FirstName = "Host", LastName = "User" },
                new UserProfile { UserId = guestUser.Id, FirstName = "Guest", LastName = "User" }
            );
            await _context.SaveChangesAsync();
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
        if (!_context.Countries.Any())
        {
            // ── Countries ──────────────────────────────────────────────
            var us = new Country { Name = "United States", Code = "US" };
            var fr = new Country { Name = "France",        Code = "FR" };
            var it = new Country { Name = "Italy",         Code = "IT" };
            var es = new Country { Name = "Spain",         Code = "ES" };
            var de = new Country { Name = "Germany",       Code = "DE" };
            var gb = new Country { Name = "United Kingdom",Code = "GB" };
            var vn = new Country { Name = "Việt Nam",      Code = "VN" };
            _context.Countries.AddRange(us, fr, it, es, de, gb, vn);
            await _context.SaveChangesAsync();

            // ── States ─────────────────────────────────────────────────
            // US
            var california = new State { Name = "California",  CountryId = us.Id };
            var newYork    = new State { Name = "New York",     CountryId = us.Id };
            var florida    = new State { Name = "Florida",      CountryId = us.Id };
            var hawaii     = new State { Name = "Hawaii",       CountryId = us.Id };
            // France
            var ileDeFrance  = new State { Name = "Île-de-France",              CountryId = fr.Id };
            var paca         = new State { Name = "Provence-Alpes-Côte d'Azur", CountryId = fr.Id };
            // Italy
            var lazio    = new State { Name = "Lazio",   CountryId = it.Id };
            var tuscany  = new State { Name = "Tuscany", CountryId = it.Id };
            var lombardy = new State { Name = "Lombardy",CountryId = it.Id };
            // Spain
            var catalonia = new State { Name = "Catalonia",           CountryId = es.Id };
            var madrid    = new State { Name = "Community of Madrid",  CountryId = es.Id };
            // Germany
            var berlin  = new State { Name = "Berlin",  CountryId = de.Id };
            var bavaria = new State { Name = "Bavaria", CountryId = de.Id };
            // UK
            var england  = new State { Name = "England",  CountryId = gb.Id };
            var scotland = new State { Name = "Scotland", CountryId = gb.Id };
            // Vietnam
            var khanhHoa = new State { Name = "Khánh Hòa", CountryId = vn.Id };
            var lamDong  = new State { Name = "Lâm Đồng",  CountryId = vn.Id };

            _context.States.AddRange(
                california, newYork, florida, hawaii,
                ileDeFrance, paca,
                lazio, tuscany, lombardy,
                catalonia, madrid,
                berlin, bavaria,
                england, scotland,
                khanhHoa, lamDong);
            await _context.SaveChangesAsync();

            // ── Cities ─────────────────────────────────────────────────
            // US
            var losAngeles   = new City { Name = "Los Angeles",   StateId = california.Id };
            var sanFrancisco = new City { Name = "San Francisco",  StateId = california.Id };
            var newYorkCity  = new City { Name = "New York City",  StateId = newYork.Id };
            var miami        = new City { Name = "Miami",          StateId = florida.Id };
            var honolulu     = new City { Name = "Honolulu",       StateId = hawaii.Id };
            // France
            var paris  = new City { Name = "Paris", StateId = ileDeFrance.Id };
            var nice   = new City { Name = "Nice",  StateId = paca.Id };
            var cannes = new City { Name = "Cannes",StateId = paca.Id };
            // Italy
            var rome     = new City { Name = "Rome",     StateId = lazio.Id };
            var florence = new City { Name = "Florence", StateId = tuscany.Id };
            var milan    = new City { Name = "Milan",    StateId = lombardy.Id };
            // Spain
            var barcelona  = new City { Name = "Barcelona", StateId = catalonia.Id };
            var madridCity = new City { Name = "Madrid",    StateId = madrid.Id };
            // Germany
            var berlinCity = new City { Name = "Berlin", StateId = berlin.Id };
            var munich     = new City { Name = "Munich", StateId = bavaria.Id };
            // UK
            var london    = new City { Name = "London",    StateId = england.Id };
            var edinburgh = new City { Name = "Edinburgh", StateId = scotland.Id };
            // Vietnam
            var camRanh = new City { Name = "Cam Ranh", StateId = khanhHoa.Id };
            var daLat   = new City { Name = "Đà Lạt",   StateId = lamDong.Id };

            _context.Cities.AddRange(
                losAngeles, sanFrancisco, newYorkCity, miami, honolulu,
                paris, nice, cannes,
                rome, florence, milan,
                barcelona, madridCity,
                berlinCity, munich,
                london, edinburgh,
                camRanh, daLat);
            await _context.SaveChangesAsync();

            // ── Locations ──────────────────────────────────────────────
            _context.Locations.AddRange(
                // US
                new Location { Address = "123 Sunset Blvd",          CityId = losAngeles.Id,   Latitude = 34.052235,  Longitude = -118.243683 },
                new Location { Address = "1 Ferry Building",          CityId = sanFrancisco.Id, Latitude = 37.795490,  Longitude = -122.393590 },
                new Location { Address = "350 5th Ave",               CityId = newYorkCity.Id,  Latitude = 40.748817,  Longitude = -73.985428  },
                new Location { Address = "1 Ocean Drive",             CityId = miami.Id,        Latitude = 25.774200,  Longitude = -80.132700  },
                new Location { Address = "2525 Kalakaua Ave",         CityId = honolulu.Id,     Latitude = 21.276440,  Longitude = -157.826180 },
                // France
                new Location { Address = "5 Avenue Anatole France",   CityId = paris.Id,        Latitude = 48.858370,  Longitude = 2.294481    },
                new Location { Address = "3 Promenade des Anglais",   CityId = nice.Id,         Latitude = 43.695990,  Longitude = 7.266070    },
                new Location { Address = "1 Bd de la Croisette",      CityId = cannes.Id,       Latitude = 43.551390,  Longitude = 7.017370    },
                // Italy
                new Location { Address = "Piazza del Colosseo 1",     CityId = rome.Id,         Latitude = 41.890251,  Longitude = 12.492373   },
                new Location { Address = "Piazza del Duomo",          CityId = florence.Id,     Latitude = 43.773080,  Longitude = 11.255840   },
                new Location { Address = "Piazza del Duomo, Milano",  CityId = milan.Id,        Latitude = 45.464210,  Longitude = 9.191630    },
                // Spain
                new Location { Address = "La Rambla 1",               CityId = barcelona.Id,    Latitude = 41.380900,  Longitude = 2.173400    },
                new Location { Address = "Puerta del Sol 1",          CityId = madridCity.Id,   Latitude = 40.416900,  Longitude = -3.703800   },
                // Germany
                new Location { Address = "Unter den Linden 1",        CityId = berlinCity.Id,   Latitude = 52.516270,  Longitude = 13.377220   },
                new Location { Address = "Marienplatz 1",             CityId = munich.Id,       Latitude = 48.137150,  Longitude = 11.575500   },
                // UK
                new Location { Address = "Westminster Bridge Rd",     CityId = london.Id,       Latitude = 51.500729,  Longitude = -0.124625   },
                new Location { Address = "1 Royal Mile",              CityId = edinburgh.Id,    Latitude = 55.948990,  Longitude = -3.199790   },
                // Vietnam
                new Location { Address = "Bãi Dài",                   CityId = camRanh.Id,      Latitude = 12.062060,  Longitude = 109.213210  },
                new Location { Address = "Hồ Tuyền Lâm",              CityId = daLat.Id,        Latitude = 11.905625,  Longitude = 108.432658  }
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

                // Thêm Review (Nối với Booking, Listing, Guest)
                var review = new Review
                {
                    BookingId = booking.Id,
                    ListingId = savedListing.Id,
                    AuthorId = guestUser.Id, // Khách đánh giá
                    OverallRating = 5,
                    Comment = "Một trải nghiệm quá tuyệt vời, chủ nhà thân thiện!"
                };
                
                // Host phản hồi lại
                review.Reply = new ReviewReply
                {
                    AuthorId = hostUser.Id, 
                    Comment = "Cảm ơn bạn rất nhiều! Rất hy vọng được đón tiếp bạn trong tương lai."
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
