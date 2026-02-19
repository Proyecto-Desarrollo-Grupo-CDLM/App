using Microsoft.EntityFrameworkCore;
using MundiFavs.Calificaciones;
using MundiFavs.Destinos;
using MundiFavs.Eventos;
using MundiFavs.Notificaciones; // 👈 1. AGREGADO: Namespace para notificaciones
using MundiFavs.ApiMetrics;
using MundiFavs.Experiencias;
using MundiFavs.Favoritos;
using System;
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
    public DbSet<Destino> Destinos { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<PreferenciaNotificacion> PreferenciasNotificaciones { get; set; }
    public DbSet<ApiMetric> ApiMetrics { get; set; }
    public DbSet<Experiencia> Experiencias { get; set; }
    

    #region Identity Entities
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
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

   
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();



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



            // [CORRECCIÓN] Convertir Uri <-> String para que SQL no falle

            b.Property(x => x.ImageUrl)

             .IsRequired()

             .HasConversion(

                 v => v.ToString(),      // De C# a Base de Datos

                 v => new Uri(v)         // De Base de Datos a C#

             );

        });

        builder.Entity<Calificacion>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Calificaciones", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasOne<IdentityUser>().WithMany().HasForeignKey(c => c.UserId).IsRequired();
            b.HasOne(c => c.Destino).WithMany().HasForeignKey(c => c.DestinoId).IsRequired();
        });

        builder.Entity<PreferenciaNotificacion>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "PreferenciasNotificaciones", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasIndex(x => new { x.UserId }).IsUnique();
        });

        builder.Entity<ApiMetric>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "ApiMetrics", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
        }); 

        builder.Entity<Experiencia>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Experiencias", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Comentario).IsRequired().HasMaxLength(2000);
            b.HasIndex(x => x.DestinoId);
            b.HasIndex(x => x.UserdId); 
        });

        builder.Entity<Evento>(b =>
        {
            b.ToTable(MundiFavsConsts.DbTablePrefix + "Eventos", MundiFavsConsts.DbSchema);
            b.ConfigureByConvention(); // Configura Id, CreationTime, etc. auto

            b.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
            b.Property(x => x.ExternalId).IsRequired().HasMaxLength(100);

            // Relación con Destino (opcional, pero recomendada si usas SQL)
            b.HasOne<Destino>().WithMany().HasForeignKey(x => x.DestinoId);
        });
    }
}
 