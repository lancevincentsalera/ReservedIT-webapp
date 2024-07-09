using System;
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
                optionsBuilder.UseSqlServer("Addr=localhost;database=ReservedIT;Integrated Security=False;Trusted_Connection=True");
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

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__Booking__RoomID__52593CB8");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Booking__UserID__5165187F");
            });

            modelBuilder.Entity<DayOfTheWeek>(entity =>
            {
                entity.HasKey(e => e.DayOfWeekId)
                    .HasName("PK__DayOfThe__01AA8DDFEA153364");

                entity.ToTable("DayOfTheWeek");

                entity.Property(e => e.DayOfWeekId)
                    .ValueGeneratedNever()
                    .HasColumnName("DayOfWeekID");

                entity.Property(e => e.DayName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
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
                    .HasName("PK__ImageGal__7516F4EC91074061");

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
                    .HasConstraintName("FK__ImageGall__RoomI__5629CD9C");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA3577276F1A1")
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

                entity.Property(e => e.BookingId).HasColumnName("BookingID");

                entity.Property(e => e.DayOfWeekId).HasColumnName("DayOfWeekID");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Recurrences)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK__Recurrenc__Booki__70DDC3D8");

                entity.HasOne(d => d.DayOfWeek)
                    .WithMany(p => p.Recurrences)
                    .HasForeignKey(d => d.DayOfWeekId)
                    .HasConstraintName("FK_Recurrence_DayOfWeek");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616059096742")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionsId)
                    .HasName("PK__RolePerm__18B281802159A9F3");

                entity.Property(e => e.RolePermissionsId).HasColumnName("RolePermissionsID");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__RolePermi__Permi__3E52440B");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__RolePermi__RoleI__3D5E1FD2");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.HasIndex(e => e.RoomName, "UQ__Room__6B500B55A8B17C07")
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

                entity.HasOne(d => d.Equipment)
                    .WithMany(p => p.RoomEquipments)
                    .HasForeignKey(d => d.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoomEquipment_Equipment");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomEquipments)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoomEquipment_Room");
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
                    .HasConstraintName("FK__Setting__UserID__5EBF139D");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__A9D105344CD5B135")
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
