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

        if (!_context.Categories.Any())
        {
            _context.Categories.AddRange(
                new Category { Name = "Nhà bên bờ biển", Icon = "beach", SortOrder = 1 },
                new Category { Name = "Biệt thự hồ bơi", Icon = "pool", SortOrder = 2 },
                new Category { Name = "Khu cắm trại", Icon = "camping", SortOrder = 3 }
            );

            await _context.SaveChangesAsync();
        }

        if (!_context.Locations.Any())
        {
            _context.Locations.AddRange(
                new Location { Address = "Bãi Dài", City = "Cam Ranh", State = "Khánh Hòa", Country = "Việt Nam", Latitude = 12.062060, Longitude = 109.213210 },
                new Location { Address = "Hồ Tuyền Lâm", City = "Đà Lạt", State = "Lâm Đồng", Country = "Việt Nam", Latitude = 11.905625, Longitude = 108.432658 }
            );

            await _context.SaveChangesAsync();
        }

        if (!_context.Listings.Any())
        {
            var category = await _context.Categories.FirstOrDefaultAsync();
            var location = await _context.Locations.FirstOrDefaultAsync();

            // Ràng buộc phải có Host User mới tạo Listing
            if (category != null && location != null && hostUser != null)
            {
                _context.Listings.Add(new Listing
                {
                    HostId = hostUser.Id, // <-- Gắn bằng User mang Role Host
                    CategoryId = category.Id,
                    LocationId = location.Id,
                    Title = "Biệt thự sát biển nhìn ra biển lộng gió",
                    Description = "Trải nghiệm kỳ nghỉ tuyệt vời cùng gia đình tại bờ biển nên thơ.",
                    PricePerNight = 1200000,
                    CleaningFee = 250000,
                    MaxGuests = 6,
                    Bedrooms = 3,
                    Beds = 4,
                    Bathrooms = 2,
                    PropertyType = "Villa",
                    InstantBook = true,
                    Status = Heven.Api.Domain.Enums.ListingStatus.Active
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
