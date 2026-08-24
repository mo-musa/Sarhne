using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sarhne.Domain.Constants;
using Sarhne.Domain.Entities.Identity;

namespace Sarhne.Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SeedAdminSettings _settings;

    private static readonly string[] RolesToSeed =
    [
        Roles.User,
        Roles.Admin,
        Roles.SuperAdmin
    ];

    public IdentitySeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<SeedAdminSettings> options)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _settings = options.Value;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedSuperAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in RolesToSeed)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await _roleManager.CreateAsync(
                new ApplicationRole
                {
                    Name = roleName
                });

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Failed to create '{roleName}' role. Errors: {errors}");
            }
        }
    }

    private async Task SeedSuperAdminAsync()
    {
        var superAdmin =
            await _userManager.FindByEmailAsync(
                _settings.Email);

        if (superAdmin is null)
        {
            superAdmin = new ApplicationUser(
                _settings.UserName,
                _settings.Email)
            {
                FullName = _settings.FullName,
                Gender = _settings.Gender,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(
                superAdmin,
                _settings.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Failed to create SuperAdmin. Errors: {errors}");
            }
        }

        if (!await _userManager.IsInRoleAsync(
                superAdmin,
                Roles.SuperAdmin))
        {
            var result = await _userManager.AddToRoleAsync(
                superAdmin,
                Roles.SuperAdmin);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Failed to assign '{Roles.SuperAdmin}' role. " +
                    $"Errors: {errors}");
            }
        }
    }
}