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
        public virtual DbSet<ImageGallery> ImageGalleries { get; set; }
        public virtual DbSet<Recurrence> Recurrences { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
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
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedDT");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Booking__RoomID__46E78A0C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Booking__UserID__47DBAE45");
            });

            modelBuilder.Entity<DayOfTheWeek>(entity =>
            {
                entity.HasKey(e => e.DayOfWeekId)
                    .HasName("PK__DayOfThe__01AA8DDFAC974931");

                entity.ToTable("DayOfTheWeek");

                entity.Property(e => e.DayOfWeekId).HasColumnName("DayOfWeekID");

                entity.Property(e => e.DayName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ImageGallery>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__ImageGal__7516F4EC333DBB58");

                entity.ToTable("ImageGallery");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ImageName).IsUnicode(false);

                entity.Property(e => e.Path).IsUnicode(false);

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.ImageGalleries)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__ImageGall__RoomI__48CFD27E");
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
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Recurrenc__Booki__49C3F6B7");

                entity.HasOne(d => d.DayOfWeek)
                    .WithMany(p => p.Recurrences)
                    .HasForeignKey(d => d.DayOfWeekId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Recurrenc__DayOf__4AB81AF0");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.HasIndex(e => e.RoomName, "UQ__Room__6B500B55BD8B0135")
                    .IsUnique();

                entity.Property(e => e.RoomId).HasColumnName("RoomID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("CreatedDT");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Equipments).IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Thumbnail)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDt)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdatedDT");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting");

                entity.Property(e => e.SettingId).HasColumnName("SettingID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Settings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Setting__UserID__4BAC3F29");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534F48FDDCF")
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
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
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
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__User__RoleID__4CA06362");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
