using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Microsoft.Extensions.Hosting;

namespace Yard_Management_System
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;

        public DbSet<PermissionRole> PermissionRoles { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=yms_db;Username=postgres;Password=13245");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<Role>()
            .HasMany(c => c.Permissions)
            .WithMany(s => s.Roles)
            .UsingEntity<PermissionRole>(
               j => j
                .HasOne(pt => pt.Permission)
                .WithMany(t => t.PermissionRoles)
                .HasForeignKey(pt => pt.PermissionId),
            j => j
                .HasOne(pt => pt.Role)
                .WithMany(p => p.PermissionRoles)
                .HasForeignKey(pt => pt.RoleId),
            j =>
            {
                j.HasKey(t => new { t.RoleId, t.PermissionId });
                j.ToTable("PermissionRoles");
            });

            Permission View = new Permission { PermissionId = 1, Name = "Смотреть пользователей", Access = true };
            Permission Delete = new Permission { PermissionId = 2, Name = "Удалять пользователей", Access = true };

            modelBuilder.Entity<Permission>().HasData(View,Delete);

            Role MainAdmin = new Role { RoleId = 1, Title = "Гл. Администратор", Permissions = new List<Permission> { View, Delete } };
            Role Receptionist = new Role { RoleId = 2, Title = "Оператор стойки регистрации", Permissions = new List<Permission> { View } };

            modelBuilder.Entity<Role>().HasData(MainAdmin,Receptionist);
        }
    }
}
