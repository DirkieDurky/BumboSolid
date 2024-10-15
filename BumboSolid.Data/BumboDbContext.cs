using BumboSolid.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BumboSolid.Data;

public partial class BumboDbContext : DbContext
{
    public BumboDbContext(DbContextOptions<BumboDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Factor> Factors { get; set; }

    public virtual DbSet<FactorType> FactorTypes { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<HolidayDay> HolidayDays { get; set; }

    public virtual DbSet<Norm> Norms { get; set; }

    public virtual DbSet<Prognosis> Prognoses { get; set; }

    public virtual DbSet<PrognosisDay> PrognosisDays { get; set; }

    public virtual DbSet<PrognosisFunction> PrognosisFunctions { get; set; }

    public virtual DbSet<Weather> Weathers { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=localhost;Database=BumboDB;Trusted_Connection=True;Encrypt=False;");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=BumboDB;trusted_connection=True;Encrypt=False");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Factor>(entity =>
        {
            entity.HasKey(e => new { e.PrognosisId, e.Type, e.Weekday });

            entity.ToTable("Factor");

            entity.Property(e => e.PrognosisId).HasColumnName("Prognosis_ID");
            entity.Property(e => e.Type)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.WeatherId).HasColumnName("Weather_ID");

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

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("Function");

            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false);
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
            entity.HasKey(e => e.HolidayName);

            entity.ToTable("HolidayDay");

            entity.Property(e => e.HolidayName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("Holiday_Name");

            entity.HasOne(d => d.HolidayNameNavigation).WithOne(p => p.HolidayDay)
                .HasForeignKey<HolidayDay>(d => d.HolidayName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HolidayDay_Holiday");
        });

        modelBuilder.Entity<Norm>(entity =>
        {
            entity.ToTable("Norm");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Activity)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Function)
                .HasMaxLength(25)
                .IsUnicode(false);

            entity.HasOne(d => d.FunctionNavigation).WithMany(p => p.Norms)
                .HasForeignKey(d => d.Function)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Norm_Function");
        });

        modelBuilder.Entity<Prognosis>(entity =>
        {
            entity.ToTable("Prognosis");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<PrognosisDay>(entity =>
        {
            entity.HasKey(e => new { e.PrognosisId, e.Weekday });

            entity.ToTable("PrognosisDay");

            entity.Property(e => e.PrognosisId).HasColumnName("Prognosis_ID");

            entity.HasOne(d => d.Prognosis).WithMany(p => p.PrognosisDays)
                .HasForeignKey(d => d.PrognosisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrognosisDay_Prognosis");
        });

        modelBuilder.Entity<PrognosisFunction>(entity =>
        {
            entity.HasKey(e => new { e.PrognosisId, e.Function, e.Weekday });

            entity.ToTable("PrognosisFunction");

            entity.Property(e => e.PrognosisId).HasColumnName("Prognosis_ID");
            entity.Property(e => e.Function)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Staff).HasColumnType("decimal(3, 2)");

            entity.HasOne(d => d.FunctionNavigation).WithMany(p => p.PrognosisFunctions)
                .HasForeignKey(d => d.Function)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrognosisFunction_Function");

            entity.HasOne(d => d.PrognosisDay).WithMany(p => p.PrognosisFunctions)
                .HasForeignKey(d => new { d.PrognosisId, d.Weekday })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrognosisFunction_PrognosisDay");
        });

        modelBuilder.Entity<Weather>(entity =>
        {
            entity.ToTable("Weather");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
