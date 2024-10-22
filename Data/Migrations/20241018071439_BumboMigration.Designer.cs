﻿// <auto-generated />
using System;
using BumboSolid.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BumboSolid.Data.Migrations
{
    [DbContext(typeof(BumboDbContext))]
    [Migration("20241018071439_BumboMigration")]
    partial class BumboMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BumboSolid.Data.Models.Factor", b =>
                {
                    b.Property<int>("PrognosisId")
                        .HasColumnType("int")
                        .HasColumnName("Prognosis_ID");

                    b.Property<string>("Type")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.Property<byte>("Weekday")
                        .HasColumnType("tinyint");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<short>("Impact")
                        .HasColumnType("smallint");

                    b.Property<byte?>("WeatherId")
                        .HasColumnType("tinyint")
                        .HasColumnName("Weather_ID");

                    b.HasKey("PrognosisId", "Type", "Weekday");

                    b.HasIndex("Type");

                    b.HasIndex("WeatherId");

                    b.HasIndex("PrognosisId", "Weekday");

                    b.ToTable("Factor", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.FactorType", b =>
                {
                    b.Property<string>("Type")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.HasKey("Type");

                    b.ToTable("FactorType", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Function", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.HasKey("Name");

                    b.ToTable("Function", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Holiday", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.HasKey("Name");

                    b.ToTable("Holiday", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.HolidayDay", b =>
                {
                    b.Property<string>("HolidayName")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)")
                        .HasColumnName("Holiday_Name");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<short>("Impact")
                        .HasColumnType("smallint");

                    b.HasKey("HolidayName", "Date");

                    b.ToTable("HolidayDay", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Norm", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("Activity")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<byte>("AvgDailyPerformances")
                        .HasColumnType("tinyint");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Function")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.Property<bool>("PerVisitor")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Function");

                    b.ToTable("Norm", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Prognosis", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<byte>("Week")
                        .HasColumnType("tinyint");

                    b.Property<short>("Year")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Prognosis", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDay", b =>
                {
                    b.Property<int>("PrognosisId")
                        .HasColumnType("int")
                        .HasColumnName("Prognosis_ID");

                    b.Property<byte>("Weekday")
                        .HasColumnType("tinyint");

                    b.Property<int>("VisitorEstimate")
                        .HasColumnType("int");

                    b.HasKey("PrognosisId", "Weekday");

                    b.ToTable("PrognosisDay", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisFunction", b =>
                {
                    b.Property<int>("PrognosisId")
                        .HasColumnType("int")
                        .HasColumnName("Prognosis_ID");

                    b.Property<string>("Function")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.Property<byte>("Weekday")
                        .HasColumnType("tinyint");

                    b.Property<decimal>("Staff")
                        .HasColumnType("decimal(3, 2)");

                    b.Property<short>("WorkHours")
                        .HasColumnType("smallint");

                    b.HasKey("PrognosisId", "Function", "Weekday");

                    b.HasIndex("Function");

                    b.HasIndex("PrognosisId", "Weekday");

                    b.ToTable("PrognosisFunction", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Weather", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint")
                        .HasColumnName("ID");

                    b.Property<short>("Impact")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Weather", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Factor", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.FactorType", "TypeNavigation")
                        .WithMany("Factors")
                        .HasForeignKey("Type")
                        .IsRequired()
                        .HasConstraintName("FK_Factor_FactorType");

                    b.HasOne("BumboSolid.Data.Models.Weather", "Weather")
                        .WithMany("Factors")
                        .HasForeignKey("WeatherId")
                        .HasConstraintName("FK_Factor_Weather");

                    b.HasOne("BumboSolid.Data.Models.PrognosisDay", "PrognosisDay")
                        .WithMany("Factors")
                        .HasForeignKey("PrognosisId", "Weekday")
                        .IsRequired()
                        .HasConstraintName("FK_Factor_PrognosisDay");

                    b.Navigation("PrognosisDay");

                    b.Navigation("TypeNavigation");

                    b.Navigation("Weather");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.HolidayDay", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Holiday", "HolidayNameNavigation")
                        .WithMany("HolidayDays")
                        .HasForeignKey("HolidayName")
                        .IsRequired()
                        .HasConstraintName("FK_HolidayDay_Holiday");

                    b.Navigation("HolidayNameNavigation");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Norm", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Function", "FunctionNavigation")
                        .WithMany("Norms")
                        .HasForeignKey("Function")
                        .IsRequired()
                        .HasConstraintName("FK_Norm_Function");

                    b.Navigation("FunctionNavigation");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDay", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Prognosis", "Prognosis")
                        .WithMany("PrognosisDays")
                        .HasForeignKey("PrognosisId")
                        .IsRequired()
                        .HasConstraintName("FK_PrognosisDay_Prognosis");

                    b.Navigation("Prognosis");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisFunction", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Function", "FunctionNavigation")
                        .WithMany("PrognosisFunctions")
                        .HasForeignKey("Function")
                        .IsRequired()
                        .HasConstraintName("FK_PrognosisFunction_Function");

                    b.HasOne("BumboSolid.Data.Models.PrognosisDay", "PrognosisDay")
                        .WithMany("PrognosisFunctions")
                        .HasForeignKey("PrognosisId", "Weekday")
                        .IsRequired()
                        .HasConstraintName("FK_PrognosisFunction_PrognosisDay");

                    b.Navigation("FunctionNavigation");

                    b.Navigation("PrognosisDay");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.FactorType", b =>
                {
                    b.Navigation("Factors");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Function", b =>
                {
                    b.Navigation("Norms");

                    b.Navigation("PrognosisFunctions");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Holiday", b =>
                {
                    b.Navigation("HolidayDays");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Prognosis", b =>
                {
                    b.Navigation("PrognosisDays");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDay", b =>
                {
                    b.Navigation("Factors");

                    b.Navigation("PrognosisFunctions");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Weather", b =>
                {
                    b.Navigation("Factors");
                });
#pragma warning restore 612, 618
        }
    }
}
