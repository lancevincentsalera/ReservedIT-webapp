using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AsiBasecodeDbContext : DbContext
    {
        public AsiBasecodeDbContext()
        {
        }

        public AsiBasecodeDbContext(DbContextOptions<AsiBasecodeDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingNotification> BookingNotifications { get; set; }
        public virtual DbSet<Models.DayOfWeek> DayOfWeeks { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Recurrence> Recurrences { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomGallery> RoomGalleries { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.BookingId).ValueGeneratedNever();

                entity.Property(e => e.BookingStatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt).HasColumnType("datetime");

                entity.HasOne(d => d.Recurrence)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RecurrenceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Recurrence");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_Room");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Booking_User");
            });

            modelBuilder.Entity<BookingNotification>(entity =>
            {
                entity.ToTable("BookingNotification");

                entity.Property(e => e.BookingNotificationId).ValueGeneratedNever();

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingNotifications)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingNotification_Booking");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.BookingNotifications)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookingNotification_Notification");
            });

            modelBuilder.Entity<Models.DayOfWeek>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DayOfWeek");

                entity.Property(e => e.DayName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DayOfWeek1).HasColumnName("DayOfWeek");

                entity.Property(e => e.RecurrenceId).HasColumnName("RecurrenceID");

                entity.HasOne(d => d.Recurrence)
                    .WithMany()
                    .HasForeignKey(d => d.RecurrenceId)
                    .HasConstraintName("FK_DayOfWeek_Recurrence");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).ValueGeneratedNever();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.NotificationType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.Property(e => e.PermissionId).ValueGeneratedNever();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Recurrence>(entity =>
            {
                entity.ToTable("Recurrence");

                entity.Property(e => e.RecurrenceId)
                    .ValueGeneratedNever()
                    .HasColumnName("RecurrenceID");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermission");

                entity.Property(e => e.RolePermissionId).ValueGeneratedNever();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Permission");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Role");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.RoomId).ValueGeneratedNever();

                entity.Property(e => e.RoomDescription).IsUnicode(false);

                entity.Property(e => e.RoomFacility).IsUnicode(false);

                entity.Property(e => e.RoomImage).IsUnicode(false);

                entity.Property(e => e.RoomInsBy)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.RoomInsDt).HasColumnType("datetime");

                entity.Property(e => e.RoomLocation).IsUnicode(false);

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RoomThumbnail).IsUnicode(false);

                entity.Property(e => e.RoomUpdBy)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.RoomUpdDt).HasColumnType("datetime");
            });

            modelBuilder.Entity<RoomGallery>(entity =>
            {
                entity.HasKey(e => e.GalleryId);

                entity.ToTable("RoomGallery");

                entity.Property(e => e.GalleryId).ValueGeneratedNever();

                entity.Property(e => e.GalleryName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.GalleryPath)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomGalleries)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoomGallery_Room");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.UserId, "IX_User");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID");

                entity.Property(e => e.AccountStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedDT");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UpdatedBy).IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");

                entity.Property(e => e.UserEmail).IsUnicode(false);

                entity.Property(e => e.UserPassword).IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
