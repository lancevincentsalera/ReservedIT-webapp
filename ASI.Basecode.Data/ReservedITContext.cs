﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class ReservedITContext : DbContext
    {
        public ReservedITContext()
        {
        }

        public ReservedITContext(DbContextOptions<ReservedITContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<DayOfTheWeek> DayOfTheWeeks { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<ImageGallery> ImageGalleries { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Recurrence> Recurrences { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomEquipment> RoomEquipments { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Addr=AJ-DEVICE1;database=ReservedIT;Integrated Security=False;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.BookingStatus)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedDT");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.RecurrenceId).HasColumnName("RecurrenceID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Recurrence)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RecurrenceId)
                    .HasConstraintName("FK__Booking__Recurre__5070F446");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Booking__RoomID__5165187F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Booking__UserID__52593CB8");
            });

            modelBuilder.Entity<DayOfTheWeek>(entity =>
            {
                entity.HasKey(e => e.DayOfWeekId)
                    .HasName("PK__DayOfThe__01AA8DDF4478C910");

                entity.ToTable("DayOfTheWeek");

                entity.Property(e => e.DayOfWeekId).HasColumnName("DayOfWeekID");

                entity.Property(e => e.DayName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RecurrenceId).HasColumnName("RecurrenceID");

                entity.HasOne(d => d.Recurrence)
                    .WithMany(p => p.DayOfTheWeeks)
                    .HasForeignKey(d => d.RecurrenceId)
                    .HasConstraintName("FK__DayOfTheW__Recur__534D60F1");
            });

            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");

                entity.Property(e => e.EquipmentName)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ImageGallery>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__ImageGal__7516F4EC55B08634");

                entity.ToTable("ImageGallery");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ImageName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Path).IsUnicode(false);

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.ImageGalleries)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__ImageGall__RoomI__5441852A");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA357ACB68A9A")
                    .IsUnique();

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.Description)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.PermissionName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Recurrence>(entity =>
            {
                entity.ToTable("Recurrence");

                entity.Property(e => e.RecurrenceId).HasColumnName("RecurrenceID");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61607EABE8B3")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionsId)
                    .HasName("PK__RolePerm__18B281802A3861CC");

                entity.Property(e => e.RolePermissionsId).HasColumnName("RolePermissionsID");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__RolePermi__Permi__5535A963");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__RolePermi__RoleI__5629CD9C");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.HasIndex(e => e.RoomName, "UQ__Room__6B500B55566A4C6A")
                    .IsUnique();

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedDT");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RoomName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Thumbnail).IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");
            });

            modelBuilder.Entity<RoomEquipment>(entity =>
            {
                entity.ToTable("RoomEquipment");

                entity.Property(e => e.RoomEquipmentId).HasColumnName("RoomEquipmentID");

                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.RoomEquipments)
                    .HasForeignKey(d => d.EquipmentId)
                    .HasConstraintName("FK__RoomEquip__Equip__571DF1D5");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomEquipments)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__RoomEquip__RoomI__5812160E");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting");

                entity.Property(e => e.SettingId).HasColumnName("SettingID");

                entity.Property(e => e.BookingReminder).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Settings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Setting__UserID__59063A47");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__A9D1053435A8CC16")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.AccountStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedDT");

                entity.Property(e => e.Email)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Users_RoleID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
