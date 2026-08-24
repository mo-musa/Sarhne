using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sarhne.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Persistence.Configurations.Identity;

public class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        
    }
}
