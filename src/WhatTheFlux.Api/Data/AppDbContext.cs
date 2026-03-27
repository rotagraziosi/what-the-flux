using Microsoft.EntityFrameworkCore;
using WhatTheFlux.Api.Models;

namespace WhatTheFlux.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Period> Periods { get; set; }
    public DbSet<DayEntry> DayEntries { get; set; }
    public DbSet<CriterionCount> CriterionCounts { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<DayEntry>()
            .HasIndex(d => new { d.PeriodId, d.DayNumber })
            .IsUnique();

        b.Entity<CriterionCount>()
            .HasIndex(c => new { c.DayEntryId, c.CriterionId })
            .IsUnique();

        b.Entity<Period>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        b.Entity<Period>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
