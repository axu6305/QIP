using Microsoft.EntityFrameworkCore;
using QIP.Web.Models;

namespace QIP.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<WardPresence> WardPresences => Set<WardPresence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(x => x.Surname).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Initials).HasMaxLength(50).IsRequired();
            entity.Property(x => x.HospitalNumber).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.HospitalNumber).IsUnique();
        });

        modelBuilder.Entity<WardPresence>(entity =>
        {
            entity.HasIndex(x => new { x.PatientId, x.Date }).IsUnique();
            entity.HasOne(x => x.Patient)
                .WithMany(x => x.WardPresences)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
