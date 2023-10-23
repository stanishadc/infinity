using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InfinityWeb.Models;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
        new IdentityRole
        {
            Name = UserRoles.Admin,
            NormalizedName = UserRoles.Admin
        },
        new IdentityRole
        {
            Name = UserRoles.Client,
            NormalizedName = UserRoles.Client
        },
        new IdentityRole
        {
            Name = UserRoles.User,
            NormalizedName = UserRoles.User
        });
    }
}