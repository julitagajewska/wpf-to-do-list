using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using ToDoListApp.MVVM.Model;

namespace ToDoListApp.Data;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ToDoDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<Subtask> Subtasks { get; set; }
    public DbSet<Planner> Planners { get; set; }
    public DbSet<MainTask> MainTasks { get; set; }
    public DbSet<Category> Categories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
        .HasOne(u => u.Planner)
        .WithOne(p => p.User)
        .HasForeignKey<UserModel>(u => u.PlannerId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Planner>()
            .HasMany(p => p.MainTasks)
            .WithOne(m => m.Planner)
            .HasForeignKey(m => m.PlannerId)
            .OnDelete(DeleteBehavior.Cascade);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
