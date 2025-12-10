using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using MundiFavs.Destinos;       
using MundiFavs.Calificaciones;


namespace MundiFavs.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class MundiFavsDbContext :
    AbpDbContext<MundiFavsDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregaate Roots / Entities here. */

    public DbSet<Destino> Destinos { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext 
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public MundiFavsDbContext(DbContextOptions<MundiFavsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        builder.Entity<Destino>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Destinos", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Nombre).IsRequired().HasMaxLength(128);
            b.Property(x => x.Pais).IsRequired().HasMaxLength(64);
            b.Property(x => x.Ciudad).IsRequired().HasMaxLength(64);
            b.OwnsOne(x => x.Ubicacion, y =>
            {
                y.Property(z => z.Latitud).IsRequired().HasColumnName("Latitud");
                y.Property(z => z.Longitud).IsRequired().HasColumnName("Longitud");
            });
            b.Property(x => x.Poblacion).IsRequired();
            b.Property(x => x.ImageUrl).IsRequired();
        });


        builder.Entity<Calificacion>(b =>
        {
            // Define el nombre de la tabla (ej: "AppCalificaciones")
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Calificaciones", MundiFavsConsts.DbSchema);

            b.ConfigureByConvention(); // Configura propiedades base (Id)

            // Configuración de propiedades
            b.Property(c => c.Estrellas).IsRequired();
            b.Property(c => c.Comentario).HasMaxLength(500); // Límite de 500 caracteres

            b.HasOne<IdentityUser>()
                        .WithMany()           // Un Usuario puede tener Muchas calificaciones
                        .HasForeignKey(c => c.UserId) // La clave foránea es UserId
                        .IsRequired();

            // Relación 1-a-Muchos con Destino
            b.HasOne(c => c.Destino)
                .WithMany() // Un Destino puede tener Muchas calificaciones
                .HasForeignKey(c => c.DestinoId) // La clave foránea es IdDestino
                .IsRequired();
        });
    }
}
