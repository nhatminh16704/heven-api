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
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
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

            if (category != null && location != null && administrator != null)
            {
                _context.Listings.Add(new Listing
                {
                    HostId = administrator.Id,
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
