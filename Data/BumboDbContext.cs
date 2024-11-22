using System;
using System.Collections.Generic;
using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Data;

public partial class BumboDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public BumboDbContext()
    {
    }

    public BumboDbContext(DbContextOptions<BumboDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailabilityRule> AvailabilityRules { get; set; }

    public virtual DbSet<CLABreakEntry> CLABreakEntries { get; set; }

    public virtual DbSet<CLAEntry> CLAEntries { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Factor> Factors { get; set; }

    public virtual DbSet<FactorType> FactorTypes { get; set; }

    public virtual DbSet<FillRequest> FillRequests { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<HolidayDay> HolidayDays { get; set; }

    public virtual DbSet<Norm> Norms { get; set; }

    public virtual DbSet<PrognosisDay> PrognosisDays { get; set; }

    public virtual DbSet<PrognosisDepartment> PrognosisDepartments { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Weather> Weathers { get; set; }

    public virtual DbSet<Week> Weeks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=BumboDB;trusted_connection=True;Encrypt=False");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AvailabilityRule>(entity =>
        {
            entity.HasKey(e => new { e.Employee, e.Date });

            entity.ToTable("AvailabilityRule");

            entity.HasOne(d => d.EmployeeNavigation).WithMany(p => p.AvailabilityRules)
                .HasForeignKey(d => d.Employee)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvailabilityRule_Employee");
        });

        modelBuilder.Entity<CLABreakEntry>(entity =>
        {
            entity.HasKey(e => new { e.CLAEntryId, e.WorkDuration });

            entity.ToTable("CLABreakEntry");

            entity.Property(e => e.CLAEntryId).HasColumnName("CLAEntryId");

            entity.HasOne(d => d.CLAEntry).WithMany(p => p.CLABreakEntries)
                .HasForeignKey(d => d.CLAEntryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CLABreakEntry_CLAEntry");
        });

        modelBuilder.Entity<CLAEntry>(entity =>
        {
            entity.ToTable("CLAEntry");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("Department");

            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.ID);

            entity.ToTable("Employee");

            entity.Property(e => e.ID)
                .ValueGeneratedNever()
                .HasColumnName("AspNetUserID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(90)
                .IsUnicode(false);
            entity.Property(e => e.PlaceOfResidence)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.StreetName)
                .HasMaxLength(45)
                .IsUnicode(false);

            entity.HasMany(d => d.Departments).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "Capability",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("Department")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Capability_Department"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("Employee")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Capability_Employee"),
                    j =>
                    {
                        j.HasKey("Employee", "Department");
                        j.ToTable("Capability");
                        j.IndexerProperty<string>("Department")
                            .HasMaxLength(25)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<Factor>(entity =>
        {
            entity.HasKey(e => new { e.PrognosisId, e.Type, e.Weekday });

            entity.ToTable("Factor");

            entity.HasIndex(e => new { e.PrognosisId, e.Weekday }, "IX_Factor_PrognosisID_Weekday");

            entity.HasIndex(e => e.Type, "IX_Factor_Type");

            entity.HasIndex(e => e.WeatherId, "IX_Factor_WeatherID");

            entity.Property(e => e.PrognosisId).HasColumnName("PrognosisID");
            entity.Property(e => e.Type)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.WeatherId).HasColumnName("WeatherID");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Factors)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factor_FactorType");

            entity.HasOne(d => d.Weather).WithMany(p => p.Factors)
                .HasForeignKey(d => d.WeatherId)
                .HasConstraintName("FK_Factor_Weather");

            entity.HasOne(d => d.PrognosisDay).WithMany(p => p.Factors)
                .HasForeignKey(d => new { d.PrognosisId, d.Weekday })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factor_PrognosisDay");
        });

        modelBuilder.Entity<FactorType>(entity =>
        {
            entity.HasKey(e => e.Type);

            entity.ToTable("FactorType");

            entity.Property(e => e.Type)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FillRequest>(entity =>
        {
            entity.ToTable("FillRequest");

            entity.HasIndex(e => e.ShiftId, "IX_FillRequest_ShiftID");

            entity.HasIndex(e => e.SubstituteEmployeeId, "IX_FillRequest_SubstituteEmployeeID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AbsentDescription)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Absent_Description");
            entity.Property(e => e.ShiftId).HasColumnName("ShiftID");
            entity.Property(e => e.SubstituteEmployeeId).HasColumnName("SubstituteEmployeeID");

            entity.HasOne(d => d.Shift).WithMany(p => p.FillRequests)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FillRequest_Shift");

            entity.HasOne(d => d.SubstituteEmployee).WithMany(p => p.FillRequests)
                .HasForeignKey(d => d.SubstituteEmployeeId)
                .HasConstraintName("FK_FillRequest_Employee");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("Holiday");

            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HolidayDay>(entity =>
        {
            entity.HasKey(e => new { e.HolidayName, e.Date });

            entity.ToTable("HolidayDay");

            entity.Property(e => e.HolidayName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Holiday_Name");

            entity.HasOne(d => d.HolidayNameNavigation).WithMany(p => p.HolidayDays)
                .HasForeignKey(d => d.HolidayName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HolidayDay_Holiday");
        });

        modelBuilder.Entity<Norm>(entity =>
        {
            entity.ToTable("Norm");

            entity.HasIndex(e => e.Department, "IX_Norm_Department");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Activity)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Department)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.HasOne(d => d.DepartmentNavigation).WithMany(p => p.Norms)
                .HasForeignKey(d => d.Department)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Norm_Department");
        });

        modelBuilder.Entity<PrognosisDay>(entity =>
        {
            entity.HasKey(e => new { e.PrognosisId, e.Weekday });

            entity.ToTable("PrognosisDay");

            entity.Property(e => e.PrognosisId).HasColumnName("PrognosisID");

            entity.HasOne(d => d.Prognosis).WithMany(p => p.PrognosisDays)
                .HasForeignKey(d => d.PrognosisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrognosisDay_Week");
        });

        modelBuilder.Entity<PrognosisDepartment>(entity =>
        {
            entity.HasKey(e => new { e.PrognosisId, e.Department, e.Weekday });

            entity.ToTable("PrognosisDepartment");

            entity.HasIndex(e => e.Department, "IX_PrognosisDepartment_Department");

            entity.HasIndex(e => new { e.PrognosisId, e.Weekday }, "IX_PrognosisDepartment_PrognosisID_Weekday");

            entity.Property(e => e.PrognosisId).HasColumnName("PrognosisID");
            entity.Property(e => e.Department)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.HasOne(d => d.DepartmentNavigation).WithMany(p => p.PrognosisDepartments)
                .HasForeignKey(d => d.Department)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrognosisDepartment_Department");

            entity.HasOne(d => d.PrognosisDay).WithMany(p => p.PrognosisDepartments)
                .HasForeignKey(d => new { d.PrognosisId, d.Weekday })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrognosisDepartment_PrognosisDay");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("Shift");

            entity.HasIndex(e => e.Department, "IX_Shift_Department");

            entity.HasIndex(e => e.WeekId, "IX_Shift_WeekID");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Department)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.ExternalEmployeeName)
                .HasMaxLength(135)
                .IsUnicode(false);
            entity.Property(e => e.WeekId).HasColumnName("WeekID");

            entity.HasOne(d => d.DepartmentNavigation).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.Department)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shift_Department");

            entity.HasOne(d => d.Week).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.WeekId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shift_Week");
        });

        modelBuilder.Entity<Weather>(entity =>
        {
            entity.ToTable("Weather");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Week>(entity =>
        {
            entity.ToTable("Week");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<Weather>().HasData(
            new Weather { Id = 0, Impact = 75 },
            new Weather { Id = 1, Impact = 50 },
            new Weather { Id = 2, Impact = 25 },
            new Weather { Id = 3, Impact = 0 },
            new Weather { Id = 4, Impact = -25 },
            new Weather { Id = 5, Impact = -50 },
            new Weather { Id = 6, Impact = -75 }
        );
        modelBuilder.Entity<FactorType>().HasData(
            new FactorType { Type = "Feestdagen" },
            new FactorType { Type = "Weer" },
            new FactorType { Type = "Overig" }
        );
        modelBuilder.Entity<Department>().HasData(
            new Department() { Name = "Kassa" },
            new Department() { Name = "Vakkenvullen" },
            new Department() { Name = "Vers" }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
