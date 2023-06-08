﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Models;

public partial class WatchContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Smartwatch> Smartwatch { get; set; }
    public DbSet<Session> Session { get; set; }
    public WatchContext()
    {
    }

    public WatchContext(DbContextOptions<WatchContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("user id=postgres;password=password;host=datadb.a5bybwhraxb4a0dk.westeurope.azurecontainer.io; database=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseOpenIddict();

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
