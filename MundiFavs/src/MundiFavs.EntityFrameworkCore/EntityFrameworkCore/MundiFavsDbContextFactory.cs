using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MundiFavs.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MundiFavsDbContextFactory : IDesignTimeDbContextFactory<MundiFavsDbContext>
{
    public MundiFavsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        MundiFavsEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<MundiFavsDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new MundiFavsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MundiFavs.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
