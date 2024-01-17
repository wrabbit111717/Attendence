using Microsoft.EntityFrameworkCore;
using Operations.Models;


namespace Operations.Data
{
    public class OperationsDbContext : DbContext
    {
        public DbSet<Area> Area { get; set; }
        public DbSet<AreaCoordinate> AreaCoordinate { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Port> Port { get; set; }
        public DbSet<Region> Region { get; set; }
        public OperationsDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Area>()
                .HasMany(_ => _.AreaCoordinates)
                .WithOne(_ => _.Area)
                .HasForeignKey(_ => _.AreaId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<AreaCoordinate>()
                .HasOne(_ => _.Area)
                .WithMany(_ => _.AreaCoordinates)
                .HasForeignKey(_ => _.AreaId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Country>()
                .HasOne(_ => _.Region)
                .WithMany(_ => _.Countries)
                .HasForeignKey(_ => _.RegionId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Country>()
                .HasMany(_ => _.Ports)
                .WithOne(_ => _.Country)
                .HasForeignKey(_ => _.CountryId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Region>()
                .HasMany(_ => _.Countries)
                .WithOne(_ => _.Region)
                .HasForeignKey(_ => _.RegionId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Region>()
                .HasMany(_ => _.Ports)
                .WithOne(_ => _.Region)
                .HasForeignKey(_ => _.RegionId)
                .HasPrincipalKey(_ => _.Id);
            builder.Entity<Region>()
                .HasOne(_ => _.Area)
                .WithMany(a => a.Regions)
                .HasForeignKey(_ => _.AreaId)
                .HasPrincipalKey(_ => _.Id);
        }

    }
}
