﻿// <auto-generated />
using System;
using BumboSolid.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BumboSolid.Migrations
{
    [DbContext(typeof(BumboDbContext))]
    partial class BumboDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BumboSolid.Data.Models.AvailabilityRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte>("Available")
                        .HasColumnType("tinyint");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("Employee")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<byte>("School")
                        .HasColumnType("tinyint");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("Employee");

                    b.ToTable("AvailabilityRule", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.CLABreakEntry", b =>
                {
                    b.Property<int>("CLAEntryId")
                        .HasColumnType("int")
                        .HasColumnName("CLAEntryId");

                    b.Property<int>("WorkDuration")
                        .HasColumnType("int");

                    b.Property<int?>("MinBreakDuration")
                        .HasColumnType("int");

                    b.HasKey("CLAEntryId", "WorkDuration");

                    b.ToTable("CLABreakEntry", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.CLAEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AgeEnd")
                        .HasColumnType("int");

                    b.Property<int?>("AgeStart")
                        .HasColumnType("int");

                    b.Property<TimeOnly?>("EarliestWorkTime")
                        .HasColumnType("time");

                    b.Property<TimeOnly?>("LatestWorkTime")
                        .HasColumnType("time");

                    b.Property<int?>("MaxAvgWeeklyWorkDurationOverFourWeeks")
                        .HasColumnType("int");

                    b.Property<int?>("MaxShiftDuration")
                        .HasColumnType("int");

                    b.Property<int?>("MaxWorkDaysPerWeek")
                        .HasColumnType("int");

                    b.Property<int?>("MaxWorkDurationPerDay")
                        .HasColumnType("int");

                    b.Property<int?>("MaxWorkDurationPerHolidayWeek")
                        .HasColumnType("int");

                    b.Property<int?>("MaxWorkDurationPerWeek")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("CLAEntry", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Department", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.HasKey("Name");

                    b.ToTable("Department", (string)null);

                    b.HasData(
                        new
                        {
                            Name = "Kassa"
                        },
                        new
                        {
                            Name = "Vakkenvullen"
                        },
                        new
                        {
                            Name = "Vers"
                        });
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<DateOnly>("BirthDate")
                        .HasColumnType("date");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateOnly>("EmployedSince")
                        .HasColumnType("date");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(45)
                        .IsUnicode(false)
                        .HasColumnType("varchar(45)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(90)
                        .IsUnicode(false)
                        .HasColumnType("varchar(90)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("PlaceOfResidence")
                        .HasMaxLength(45)
                        .IsUnicode(false)
                        .HasColumnType("varchar(45)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetName")
                        .HasMaxLength(45)
                        .IsUnicode(false)
                        .HasColumnType("varchar(45)");

                    b.Property<int?>("StreetNumber")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Factor", b =>
                {
                    b.Property<int>("PrognosisId")
                        .HasColumnType("int")
                        .HasColumnName("PrognosisID");

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
                        .HasColumnName("WeatherID");

                    b.HasKey("PrognosisId", "Type", "Weekday");

                    b.HasIndex(new[] { "PrognosisId", "Weekday" }, "IX_Factor_PrognosisID_Weekday");

                    b.HasIndex(new[] { "Type" }, "IX_Factor_Type");

                    b.HasIndex(new[] { "WeatherId" }, "IX_Factor_WeatherID");

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

                    b.HasData(
                        new
                        {
                            Type = "Feestdagen"
                        },
                        new
                        {
                            Type = "Weer"
                        },
                        new
                        {
                            Type = "Overig"
                        });
                });

            modelBuilder.Entity("BumboSolid.Data.Models.FillRequest", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("AbsentDescription")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Absent_Description");

                    b.Property<byte>("Accepted")
                        .HasColumnType("tinyint");

                    b.Property<int>("ShiftId")
                        .HasColumnType("int")
                        .HasColumnName("ShiftID");

                    b.Property<int?>("SubstituteEmployeeId")
                        .HasColumnType("int")
                        .HasColumnName("SubstituteEmployeeID");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ShiftId" }, "IX_FillRequest_ShiftID");

                    b.HasIndex(new[] { "SubstituteEmployeeId" }, "IX_FillRequest_SubstituteEmployeeID");

                    b.ToTable("FillRequest", (string)null);
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

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<bool>("PerVisitor")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Department" }, "IX_Norm_Department");

                    b.ToTable("Norm", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDay", b =>
                {
                    b.Property<int>("PrognosisId")
                        .HasColumnType("int")
                        .HasColumnName("PrognosisID");

                    b.Property<byte>("Weekday")
                        .HasColumnType("tinyint");

                    b.Property<int>("VisitorEstimate")
                        .HasColumnType("int");

                    b.HasKey("PrognosisId", "Weekday");

                    b.ToTable("PrognosisDay", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDepartment", b =>
                {
                    b.Property<int>("PrognosisId")
                        .HasColumnType("int")
                        .HasColumnName("PrognosisID");

                    b.Property<string>("Department")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.Property<byte>("Weekday")
                        .HasColumnType("tinyint");

                    b.Property<short>("WorkHours")
                        .HasColumnType("smallint");

                    b.HasKey("PrognosisId", "Department", "Weekday");

                    b.HasIndex(new[] { "Department" }, "IX_PrognosisDepartment_Department");

                    b.HasIndex(new[] { "PrognosisId", "Weekday" }, "IX_PrognosisDepartment_PrognosisID_Weekday");

                    b.ToTable("PrognosisDepartment", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Shift", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.Property<int?>("Employee")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<string>("ExternalEmployeeName")
                        .HasMaxLength(135)
                        .IsUnicode(false)
                        .HasColumnType("varchar(135)");

                    b.Property<byte>("IsBreak")
                        .HasColumnType("tinyint");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("WeekId")
                        .HasColumnType("int")
                        .HasColumnName("WeekID");

                    b.Property<byte>("Weekday")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Department" }, "IX_Shift_Department");

                    b.HasIndex(new[] { "WeekId" }, "IX_Shift_WeekID");

                    b.ToTable("Shift", (string)null);
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

                    b.HasData(
                        new
                        {
                            Id = (byte)0,
                            Impact = (short)75
                        },
                        new
                        {
                            Id = (byte)1,
                            Impact = (short)50
                        },
                        new
                        {
                            Id = (byte)2,
                            Impact = (short)25
                        },
                        new
                        {
                            Id = (byte)3,
                            Impact = (short)0
                        },
                        new
                        {
                            Id = (byte)4,
                            Impact = (short)-25
                        },
                        new
                        {
                            Id = (byte)5,
                            Impact = (short)-50
                        },
                        new
                        {
                            Id = (byte)6,
                            Impact = (short)-75
                        });
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Week", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasColumnName("ID");

                    b.Property<byte>("WeekNumber")
                        .HasColumnType("tinyint");

                    b.Property<short>("Year")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("Week", (string)null);
                });

            modelBuilder.Entity("Capability", b =>
                {
                    b.Property<int>("Employee")
                        .HasColumnType("int");

                    b.Property<string>("Department")
                        .HasMaxLength(25)
                        .IsUnicode(false)
                        .HasColumnType("varchar(25)");

                    b.HasKey("Employee", "Department");

                    b.HasIndex("Department");

                    b.ToTable("Capability", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BumboSolid.Data.Models.AvailabilityRule", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Employee", "EmployeeNavigation")
                        .WithMany("AvailabilityRules")
                        .HasForeignKey("Employee")
                        .IsRequired()
                        .HasConstraintName("FK_AvailabilityRule_Employee");

                    b.Navigation("EmployeeNavigation");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.CLABreakEntry", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.CLAEntry", "CLAEntry")
                        .WithMany("CLABreakEntries")
                        .HasForeignKey("CLAEntryId")
                        .IsRequired()
                        .HasConstraintName("FK_CLABreakEntry_CLAEntry");

                    b.Navigation("CLAEntry");
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

            modelBuilder.Entity("BumboSolid.Data.Models.FillRequest", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Shift", "Shift")
                        .WithMany("FillRequests")
                        .HasForeignKey("ShiftId")
                        .IsRequired()
                        .HasConstraintName("FK_FillRequest_Shift");

                    b.HasOne("BumboSolid.Data.Models.Employee", "SubstituteEmployee")
                        .WithMany("FillRequests")
                        .HasForeignKey("SubstituteEmployeeId")
                        .HasConstraintName("FK_FillRequest_Employee");

                    b.Navigation("Shift");

                    b.Navigation("SubstituteEmployee");
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
                    b.HasOne("BumboSolid.Data.Models.Department", "DepartmentNavigation")
                        .WithMany("Norms")
                        .HasForeignKey("Department")
                        .IsRequired()
                        .HasConstraintName("FK_Norm_Department");

                    b.Navigation("DepartmentNavigation");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDay", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Week", "Prognosis")
                        .WithMany("PrognosisDays")
                        .HasForeignKey("PrognosisId")
                        .IsRequired()
                        .HasConstraintName("FK_PrognosisDay_Week");

                    b.Navigation("Prognosis");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDepartment", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Department", "DepartmentNavigation")
                        .WithMany("PrognosisDepartments")
                        .HasForeignKey("Department")
                        .IsRequired()
                        .HasConstraintName("FK_PrognosisDepartment_Department");

                    b.HasOne("BumboSolid.Data.Models.PrognosisDay", "PrognosisDay")
                        .WithMany("PrognosisDepartments")
                        .HasForeignKey("PrognosisId", "Weekday")
                        .IsRequired()
                        .HasConstraintName("FK_PrognosisDepartment_PrognosisDay");

                    b.Navigation("DepartmentNavigation");

                    b.Navigation("PrognosisDay");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Shift", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Department", "DepartmentNavigation")
                        .WithMany("Shifts")
                        .HasForeignKey("Department")
                        .IsRequired()
                        .HasConstraintName("FK_Shift_Department");

                    b.HasOne("BumboSolid.Data.Models.Week", "Week")
                        .WithMany("Shifts")
                        .HasForeignKey("WeekId")
                        .IsRequired()
                        .HasConstraintName("FK_Shift_Week");

                    b.Navigation("DepartmentNavigation");

                    b.Navigation("Week");
                });

            modelBuilder.Entity("Capability", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Department", null)
                        .WithMany()
                        .HasForeignKey("Department")
                        .IsRequired()
                        .HasConstraintName("FK_Capability_Department");

                    b.HasOne("BumboSolid.Data.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("Employee")
                        .IsRequired()
                        .HasConstraintName("FK_Capability_Employee");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BumboSolid.Data.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("BumboSolid.Data.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BumboSolid.Data.Models.CLAEntry", b =>
                {
                    b.Navigation("CLABreakEntries");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Department", b =>
                {
                    b.Navigation("Norms");

                    b.Navigation("PrognosisDepartments");

                    b.Navigation("Shifts");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Employee", b =>
                {
                    b.Navigation("AvailabilityRules");

                    b.Navigation("FillRequests");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.FactorType", b =>
                {
                    b.Navigation("Factors");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Holiday", b =>
                {
                    b.Navigation("HolidayDays");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.PrognosisDay", b =>
                {
                    b.Navigation("Factors");

                    b.Navigation("PrognosisDepartments");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Shift", b =>
                {
                    b.Navigation("FillRequests");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Weather", b =>
                {
                    b.Navigation("Factors");
                });

            modelBuilder.Entity("BumboSolid.Data.Models.Week", b =>
                {
                    b.Navigation("PrognosisDays");

                    b.Navigation("Shifts");
                });
#pragma warning restore 612, 618
        }
    }
}
