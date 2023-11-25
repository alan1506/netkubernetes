using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetKubernetes.Models;

namespace NetKubernetes.Data;

public class AppDbContext : IdentityDbContext<UsuarioModel>{

    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt){

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UsuarioModel>().Property(x => x.Id).HasMaxLength(36);
        builder.Entity<UsuarioModel>().Property(x => x.NormalizedUserName).HasMaxLength(90);
        builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(36);
        builder.Entity<IdentityRole>().Property(x => x.NormalizedName).HasMaxLength(36);
    }

    public DbSet<InmuebleModel>? InmuebleModel { get; set; }
}