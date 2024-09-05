using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity;

public class SeedAdminWithRolesinitialData
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext context;

    public SeedAdminWithRolesinitialData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        this.context = context;
    }
 

    public async Task SeedData()
    {
        if (_roleManager.Roles.Any())
        {
            return;
        }

        var UserRole = new IdentityRole()
        {
            Id = "1",
            Name = "Customer",
            NormalizedName = "CUSTOMER",
            ConcurrencyStamp = ""
        };
        var AdminRole = new IdentityRole()
        {
            Id = "2",

            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = ""
        };
        await _roleManager.CreateAsync(UserRole);
        await _roleManager.CreateAsync(AdminRole);

        var UserToSeed = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "Admin@gmail.com",
            UserName = "Admin@gmail.com",
            EmailConfirmed = true,
        };
        var Result = await _userManager.CreateAsync(UserToSeed, "safa1234567");

        if (Result.Succeeded)
        {
          
            await _userManager.AddToRolesAsync(UserToSeed, new List<string> { UserRole.Name, AdminRole.Name });
        }

        var customer = new Customer()
        {
            UuserID = UserToSeed.Id,
            Email=UserToSeed.Email,
            Address="Cairo",
            Age=25,
            FirstName="Safa",
            LastName="Mohamed",
            Phone="145213658"
        
        };

        context.Add(customer);
        await context.SaveChangesAsync();

  
    }
}