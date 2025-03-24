﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MinimalApis.Domain;
using MinimalApis.Domain.Accounts;

namespace MinimalApis.Database;

public class EFContext
    : DbContext, IDbContext
{
    public EFContext(DbContextOptions<EFContext> options)
        : base(options)
    { }
    public virtual DbSet<Account> Accounts { get; set; }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(i => i.Phone, "idx_accounts_phone");
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        });
        base.OnModelCreating(modelBuilder);
    }
}
