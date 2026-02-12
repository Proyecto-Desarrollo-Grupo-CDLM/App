using Microsoft.EntityFrameworkCore;
using MundiFavs.Calificaciones;
using MundiFavs.Destinos;
using MundiFavs.Eventos;
using MundiFavs.Notificaciones; // 👈 1. AGREGADO: Namespace para notificaciones
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
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace MundiFavs.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class MundiFavsDbContext :
    AbpDbContext<MundiFavsDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Destino> Destinos { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<Evento> Eventos { get; set; } // Ya lo tenías, ¡bien!

    // 👇 2. AGREGADO: Las tablas de Notificaciones
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<PreferenciaNotificacion> PreferenciasNotificaciones { get; set; }


    #region Entities from the modules

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

        // --- DESTINOS ---
        builder.Entity<Destino>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Destinos", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
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

        // --- CALIFICACIONES (Tu configuración actual) ---
        builder.Entity<Calificacion>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Calificaciones", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(c => c.Estrellas).IsRequired();
            b.Property(c => c.Comentario).HasMaxLength(500);

            b.HasOne<IdentityUser>()
                        .WithMany()
                        .HasForeignKey(c => c.UserId)
                        .IsRequired();

            b.HasOne(c => c.Destino)
                .WithMany()
                .HasForeignKey(c => c.DestinoId)
                .IsRequired();
        });

        // 👇 3. AGREGADO: CONFIGURACIONES NUEVAS

        // --- EVENTOS ---
        builder.Entity<Evento>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Eventos", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.DestinoId); // Para buscar eventos por destino rápido
        });

        // --- NOTIFICACIONES ---
        builder.Entity<Notificacion>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Notificaciones", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.UsuarioId); // Para cargar la "Bandeja de Entrada" rápido
        });

        // --- PREFERENCIAS ---
        builder.Entity<PreferenciaNotificacion>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "PreferenciasNotificaciones", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasIndex(x => x.UserId);
        });
    }
}