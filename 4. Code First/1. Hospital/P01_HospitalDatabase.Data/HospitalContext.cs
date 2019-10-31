using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;
using System;

namespace P01_HospitalDatabase.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }
        public HospitalContext(DbContextOptions options)
        {

        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<PatientMedicament> PatientsMedicaments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patients");

                entity.HasKey(e => e.PatientId);

                entity.Property(e => e.FirstName).HasMaxLength(50).IsUnicode(true);

                entity.Property(e => e.LastName).HasMaxLength(50).IsUnicode(true);

                entity.Property(e => e.Address).HasMaxLength(250).IsUnicode(true);

                entity.Property(e => e.Email).HasMaxLength(80).IsUnicode(false);
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity.ToTable("Visitations");

                entity.HasKey(e => e.VisitationId);

                entity.Property(e => e.Comments).HasMaxLength(250).IsUnicode(true);

                entity.HasOne(e => e.Patient)
                .WithMany(p => p.Visitations)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Visitations_Patients");
            });

            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity.ToTable("Diagnoses");

                entity.HasKey(e => e.DiagnoseId);

                entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(true);

                entity.Property(e => e.Comments).HasMaxLength(250).IsUnicode(true);

                entity.HasOne(e => e.Patient).WithMany(p => p.Diagnoses)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Diagnoses_Patients");
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.ToTable("Medicaments");

                entity.HasKey(e => e.MedicamentId);

                entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(true);
            });

            modelBuilder.Entity<PatientMedicament>(entity =>
            {
                entity.HasKey(e => new {e.PatientId, e.MedicamentId });

                entity.HasOne(e => e.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PatientMedicaments_Patients");

                entity.HasOne(e => e.Medicament).WithMany(m => m.Prescriptions)
                .HasForeignKey(e => e.MedicamentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PatientMedicaments_Medicaments");
            });
        }
    }
}
