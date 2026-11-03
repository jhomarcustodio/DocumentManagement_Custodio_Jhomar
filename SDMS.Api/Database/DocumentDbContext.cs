using Microsoft.EntityFrameworkCore;
using SDMS.Entities;

namespace SDMS.Database;

public sealed class DocumentDbContext(DbContextOptions<DocumentDbContext> options): DbContext(options)
{
    public DbSet<Document> Documents {get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OriginalFilename)
                .HasMaxLength(255);

            entity.Property(e => e.StoredFilename)
                .HasMaxLength(500);

            entity.Property(e => e.ContentType)
                .HasMaxLength(100);
        });
    }
}