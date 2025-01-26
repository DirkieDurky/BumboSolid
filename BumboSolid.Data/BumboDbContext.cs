using BumboSolid.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BumboSolid.Data;

public partial class BumboDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public BumboDbContext()
    {
    }

    public BumboDbContext(DbContextOptions<BumboDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailabilityRule> AvailabilityRules { get; set; }

    public virtual DbSet<Absence> Absences { get; set; }

    public virtual DbSet<CLASurchargeEntry> CLASurchargeEntries { get; set; }

    public virtual DbSet<CLAEntry> CLAEntries { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<User> Employees { get; set; }

    public virtual DbSet<Factor> Factors { get; set; }

    public virtual DbSet<FactorType> FactorTypes { get; set; }

    public virtual DbSet<FillRequest> FillRequests { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<HolidayDay> HolidayDays { get; set; }

    public virtual DbSet<Norm> Norms { get; set; }

    public virtual DbSet<PrognosisDay> PrognosisDays { get; set; }

    public virtual DbSet<PrognosisDepartment> PrognosisDepartments { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ClockedHours> ClockedHours { get; set; }

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
            entity.HasKey(e => e.Id);

            entity.ToTable("AvailabilityRule");

            entity.HasOne(d => d.EmployeeNavigation).WithMany(p => p.AvailabilityRules)
                .HasForeignKey(d => d.Employee)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AvailabilityRule_Employee");
        });

        modelBuilder.Entity<Absence>(entity =>
        {
            entity.HasKey("Id");

            entity.ToTable("Absence");

            entity.HasIndex(e => e.WeekId, "IX_Absence_WeekID");

            entity.HasIndex(e => e.EmployeeId, "IX_Absence_EmployeeID");

            entity.Property(e => e.WeekId).HasColumnName("WeekId");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

            entity.HasOne(d => d.Week).WithMany(p => p.Absences)
                .HasForeignKey(d => d.WeekId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Absence_Week");

            entity.HasOne(d => d.Employee).WithMany(p => p.Absences)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Absence_Employee")
                .OnDelete(DeleteBehavior.Cascade);
        });

		modelBuilder.Entity<CLASurchargeEntry>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.ToTable("CLASurchargeEntry");
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("User");

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
                .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_Capability_Department"),
                    l => l.HasOne<User>().WithMany()
                .HasForeignKey("Employee")
                .OnDelete(DeleteBehavior.Cascade)
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
            entity.HasKey("Id");

            entity.ToTable("FillRequest");

            entity.HasIndex(e => e.ShiftId, "IX_FillRequest_ShiftID");

            entity.HasIndex(e => e.SubstituteEmployeeId, "IX_FillRequest_SubstituteEmployeeID");

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

            entity.Property(e => e.EmployeeId).HasColumnName("Employee");
            entity.Property(e => e.Weekday).HasColumnName("Weekday");
            entity.Property(e => e.StartTime).HasColumnName("StartTime");
            entity.Property(e => e.EndTime).HasColumnName("EndTime");
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

            entity.HasOne(d => d.Employee).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Shift_Employee");
        });

        modelBuilder.Entity<ClockedHours>(entity =>
        {
            entity.ToTable("ClockedHours");

            entity.HasIndex(e => e.Department, "IX_ClockedHours_Department");

            entity.HasIndex(e => e.WeekId, "IX_ClockedHours_WeekID");

            entity.Property(e => e.EmployeeId).HasColumnName("Employee");
            entity.Property(e => e.Weekday).HasColumnName("Weekday");
            entity.Property(e => e.StartTime).HasColumnName("StartTime");
            entity.Property(e => e.EndTime).HasColumnName("EndTime");
            entity.Property(e => e.Department)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.ExternalEmployeeName)
                .HasMaxLength(135)
                .IsUnicode(false);
            entity.Property(e => e.WeekId).HasColumnName("WeekID");

            entity.HasOne(d => d.DepartmentNavigation).WithMany(p => p.ClockedHours)
                .HasForeignKey(d => d.Department)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClockedHours_Department");

            entity.HasOne(d => d.Week).WithMany(p => p.ClockedHours)
                .HasForeignKey(d => d.WeekId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClockedHours_Week");

            entity.HasOne(d => d.Employee).WithMany(p => p.ClockedHours)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ClockedHours_Employee");
        });

        modelBuilder.Entity<Weather>(entity =>
        {
            entity.ToTable("Weather");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<Week>(entity =>
        {
            entity.ToTable("Week");
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

        //Seeddata
        modelBuilder.Entity<FactorType>().HasData(
            new FactorType { Type = "Feestdagen" },
            new FactorType { Type = "Weer" },
            new FactorType { Type = "Overig" }
        );

        modelBuilder.Entity<Department>().HasData(
            new Department()
            {
                Name = "Kassa",
            },
            new Department() { Name = "Vakkenvullen" },
            new Department() { Name = "Vers" }
        );

        modelBuilder.Entity<Norm>().HasData(
            new Norm { Id = 1, Activity = "1 vak vullen", Duration = 300, AvgDailyPerformances = 5, Department = "Vakkenvullen" },
            new Norm { Id = 2, Activity = "Kassa", Duration = 120, AvgDailyPerformances = 1, PerVisitor = true, Department = "Kassa" },
            new Norm { Id = 3, Activity = "Vers", Duration = 45, AvgDailyPerformances = 8, Department = "Vers" }
        );

        modelBuilder.Entity<Week>().HasData(
            new Week { Id = 1, Year = 2024, WeekNumber = 50 },
            new Week { Id = 2, Year = 2025, WeekNumber = 1 },
            new Week { Id = 3, Year = 2025, WeekNumber = 2 },
            new Week { Id = 4, Year = 2025, WeekNumber = 3 },
            new Week { Id = 5, Year = 2025, WeekNumber = 4 },
            new Week { Id = 6, Year = 2025, WeekNumber = 5 }
        );

        modelBuilder.Entity<PrognosisDay>().HasData(
            new PrognosisDay { PrognosisId = 1, Weekday = 0, VisitorEstimate = 1000 },
            new PrognosisDay { PrognosisId = 1, Weekday = 1, VisitorEstimate = 1000 },
            new PrognosisDay { PrognosisId = 1, Weekday = 2, VisitorEstimate = 1000 },
            new PrognosisDay { PrognosisId = 1, Weekday = 3, VisitorEstimate = 1000 },
            new PrognosisDay { PrognosisId = 1, Weekday = 4, VisitorEstimate = 1000 },
            new PrognosisDay { PrognosisId = 1, Weekday = 5, VisitorEstimate = 1000 },
            new PrognosisDay { PrognosisId = 1, Weekday = 6, VisitorEstimate = 1000 }
        );
        modelBuilder.Entity<PrognosisDepartment>().HasData(
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 0, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 0, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 0, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 1, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 1, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 1, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 2, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 2, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 2, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 3, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 3, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 3, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 4, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 4, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 4, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 5, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 5, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 5, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Kassa", Weekday = 6, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vakkenvullen", Weekday = 6, WorkHours = 4000 },
            new PrognosisDepartment { PrognosisId = 1, Department = "Vers", Weekday = 6, WorkHours = 4000 }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
