using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FormDB.Models;

public partial class FormDbContext : DbContext
{
    public FormDbContext(DbContextOptions<FormDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbAuth> TbAuths { get; set; }

    public virtual DbSet<TbLog> TbLogs { get; set; }

    public virtual DbSet<TbMenu> TbMenus { get; set; }

    public virtual DbSet<TbOrgDept> TbOrgDepts { get; set; }

    public virtual DbSet<TbOrgDeptUser> TbOrgDeptUsers { get; set; }

    public virtual DbSet<TbOrgRole> TbOrgRoles { get; set; }

    public virtual DbSet<TbOrgUser> TbOrgUsers { get; set; }

    public virtual DbSet<TbRefreshToken> TbRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<TbAuth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("TbAuth");

            entity.HasIndex(e => e.MenuId, "MenuId");

            entity.Property(e => e.MenuId).HasMaxLength(50);
            entity.Property(e => e.TargetId).HasMaxLength(50);

            entity.HasOne(d => d.Menu).WithMany(p => p.TbAuths)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TbAuth_ibfk_2");
        });

        modelBuilder.Entity<TbLog>(entity =>
        {
            entity.HasKey(e => e.AutoId).HasName("PRIMARY");

            entity.ToTable("TbLog");

            entity.HasIndex(e => e.LogId, "Idx_TbLog_LogId");

            entity.Property(e => e.CurrentValues).HasColumnType("json");
            entity.Property(e => e.LogId).HasMaxLength(50);
            entity.Property(e => e.LogState).HasMaxLength(50);
            entity.Property(e => e.OriginalValues).HasColumnType("json");
            entity.Property(e => e.Uid).HasMaxLength(50);
            entity.Property(e => e.UpdateTable).HasMaxLength(50);
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<TbMenu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PRIMARY");

            entity.ToTable("TbMenu");

            entity.Property(e => e.MenuId).HasMaxLength(50);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.MenuName).HasMaxLength(50);
            entity.Property(e => e.ParentMenuId).HasMaxLength(50);
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<TbOrgDept>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PRIMARY");

            entity.ToTable("TbOrgDept");

            entity.HasIndex(e => e.ParentDeptId, "Idx_ParentDeptId");

            entity.HasIndex(e => e.RootDeptId, "Idx_RootDeptId");

            entity.HasIndex(e => new { e.RootDeptId, e.DeptId }, "Idx_RootDeptId_DeptId");

            entity.Property(e => e.DeptId).HasMaxLength(50);
            entity.Property(e => e.DeptName).HasMaxLength(50);
            entity.Property(e => e.LogId).HasMaxLength(50);
            entity.Property(e => e.ParentDeptId).HasMaxLength(50);
            entity.Property(e => e.RootDeptId).HasMaxLength(50);
        });

        modelBuilder.Entity<TbOrgDeptUser>(entity =>
        {
            entity.HasKey(e => new { e.DeptId, e.Uid })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("TbOrgDeptUser");

            entity.HasIndex(e => e.DeptId, "DeptId");

            entity.HasIndex(e => e.Uid, "Uid");

            entity.Property(e => e.DeptId).HasMaxLength(50);
            entity.Property(e => e.Uid).HasMaxLength(50);

            entity.HasOne(d => d.Dept).WithMany(p => p.TbOrgDeptUsers)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TbOrgDeptUser_ibfk_1");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.TbOrgDeptUsers)
                .HasForeignKey(d => d.Uid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TbOrgDeptUser_ibfk_2");
        });

        modelBuilder.Entity<TbOrgRole>(entity =>
        {
            entity.HasKey(e => e.Rid).HasName("PRIMARY");

            entity.ToTable("TbOrgRole");

            entity.Property(e => e.Rid).HasMaxLength(50);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<TbOrgUser>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PRIMARY");

            entity.ToTable("TbOrgUser");

            entity.HasIndex(e => e.Email, "EMail").IsUnique();

            entity.Property(e => e.Uid).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMail");
            entity.Property(e => e.LogId).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OauthProvIder)
                .HasMaxLength(50)
                .HasColumnName("OAuthProvIder");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhotoUrl).HasMaxLength(100);

            entity.HasMany(d => d.Rids).WithMany(p => p.Uids)
                .UsingEntity<Dictionary<string, object>>(
                    "TbOrgRoleUser",
                    r => r.HasOne<TbOrgRole>().WithMany()
                        .HasForeignKey("Rid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("TbOrgRoleUser_ibfk_2"),
                    l => l.HasOne<TbOrgUser>().WithMany()
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("TbOrgRoleUser_ibfk_1"),
                    j =>
                    {
                        j.HasKey("Uid", "Rid")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("TbOrgRoleUser");
                        j.HasIndex(new[] { "Rid" }, "Rid");
                        j.IndexerProperty<string>("Uid").HasMaxLength(50);
                        j.IndexerProperty<string>("Rid").HasMaxLength(50);
                    });
        });

        modelBuilder.Entity<TbRefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshToken).HasName("PRIMARY");

            entity.ToTable("TbRefreshToken");

            entity.Property(e => e.ExpireTime).HasColumnType("datetime");
            entity.Property(e => e.Uid).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
