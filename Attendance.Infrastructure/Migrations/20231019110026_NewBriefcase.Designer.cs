﻿// <auto-generated />
using System;
using Attendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Attendance.Infrastructure.Migrations
{
    [DbContext(typeof(AttendanceContext))]
    [Migration("20231019110026_NewBriefcase")]
    partial class NewBriefcase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Attendance.Models.Briefcase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyRepresentativeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InspectionCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<int>("InspectionSourceId")
                        .HasColumnType("int");

                    b.Property<string>("InspectionSourceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InspectionTypeId")
                        .HasColumnType("int");

                    b.Property<string>("InspectorName")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("PortCountry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PortId")
                        .HasColumnType("int");

                    b.Property<string>("PortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Sent")
                        .HasColumnType("bit");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasMaxLength(450);

                    b.Property<int>("VesselId")
                        .HasColumnType("int");

                    b.Property<DateTime>("VettingDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("InspectionTypeId");

                    b.HasIndex("UserId");

                    b.HasIndex("VesselId");

                    b.ToTable("Briefcase");
                });

            modelBuilder.Entity("Attendance.Models.BriefcaseQuestionnaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BriefcaseId")
                        .HasColumnType("int");

                    b.Property<int>("QId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BriefcaseId");

                    b.HasIndex("QId", "BriefcaseId")
                        .IsUnique();

                    b.ToTable("BriefcaseQuestionnaires");
                });

            modelBuilder.Entity("Attendance.Models.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CategoryNewID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Children")
                        .HasColumnType("int");

                    b.HasKey("CategoryID");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Attendance.Models.InspectionTypes", b =>
                {
                    b.Property<int>("InspectionTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("InspectionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InspectionType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("bitReport")
                        .HasColumnType("bit");

                    b.HasKey("InspectionTypeId");

                    b.ToTable("InspectionTypes");
                });

            modelBuilder.Entity("Attendance.Models.QuestionPool", b =>
                {
                    b.Property<Guid>("questionid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CategoryCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CategoryID")
                        .HasColumnType("int");

                    b.Property<Guid?>("CategoryNewID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Origin")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("QuestionTypeID")
                        .HasColumnType("int");

                    b.Property<string>("comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("question")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("questioncode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("questionid");

                    b.HasIndex("ParentId");

                    b.ToTable("QuestionPoolNew");
                });

            modelBuilder.Entity("Attendance.Models.QuestionType", b =>
                {
                    b.Property<int>("TypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TypeCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeDescription")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TypeId");

                    b.ToTable("QuestionTypes");
                });

            modelBuilder.Entity("Attendance.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(128)")
                        .HasMaxLength(128);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Attendance.Models.UserQuestionnaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("QId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QId");

                    b.HasIndex("UserId", "QId")
                        .IsUnique()
                        .HasName("IX_UserQuestionnaire_User_Q");

                    b.ToTable("UserQuestionnaire");
                });

            modelBuilder.Entity("Attendance.Models.VIQ", b =>
                {
                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("Children")
                        .HasColumnType("int");

                    b.Property<Guid?>("CommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DisplayIndex")
                        .HasColumnType("int");

                    b.Property<int?>("DisplayLevel")
                        .HasColumnType("int");

                    b.Property<int?>("GlobalDisplayIndex")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("InternalDisplayCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ObjectType")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentCategory")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ParentType")
                        .HasColumnType("int");

                    b.Property<int>("QId")
                        .HasColumnType("int");

                    b.Property<Guid?>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ShowAfterId")
                        .HasColumnType("int");

                    b.ToTable("VIQ");
                });

            modelBuilder.Entity("Attendance.Models.VIQInfoModel", b =>
                {
                    b.Property<int>("QId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EffectiveDate")
                        .HasColumnType("datetime2");

                    b.Property<byte?>("Finalized")
                        .HasColumnType("tinyint");

                    b.Property<int?>("NumOfQuestions")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("SecurityColumn")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("VIQGUI")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("Visible")
                        .HasColumnType("bit");

                    b.Property<string>("version")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QId");

                    b.ToTable("VIQInfo");
                });

            modelBuilder.Entity("Attendance.Models.Vessel", b =>
                {
                    b.Property<int>("VesselId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("DeadWeight")
                        .HasColumnType("float");

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FLAG")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FleetId")
                        .HasColumnType("int");

                    b.Property<double?>("GrossTonage")
                        .HasColumnType("float");

                    b.Property<string>("IMO")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Locked")
                        .HasColumnType("int");

                    b.Property<string>("VesselCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VesselName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VesselId");

                    b.ToTable("Vessel");
                });

            modelBuilder.Entity("Attendance.Models.Vetting", b =>
                {
                    b.Property<int>("VETId")
                        .HasColumnType("int");

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte?>("AssigneeRank")
                        .HasColumnType("tinyint");

                    b.Property<int?>("VerifiedinVetting")
                        .HasColumnType("int");

                    b.Property<string>("answer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("significance")
                        .HasColumnType("int");

                    b.HasKey("VETId", "ObjectId");

                    b.ToTable("Vetting");
                });

            modelBuilder.Entity("Attendance.Models.VettingAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ObjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("VETId")
                        .HasColumnType("int");

                    b.Property<byte[]>("commentFile")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("commentFileName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("VETId", "ObjectId");

                    b.ToTable("VettingAttachment");
                });

            modelBuilder.Entity("Attendance.Models.VettingInfo", b =>
                {
                    b.Property<int>("VetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Answered")
                        .HasColumnType("int");

                    b.Property<byte?>("CarriedOutStatus")
                        .HasColumnType("tinyint");

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyRepresentativeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<int>("InspectionTypeId")
                        .HasColumnType("int");

                    b.Property<string>("InspectorName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InspectorSirName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MajorId")
                        .HasColumnType("int");

                    b.Property<int?>("Negative")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Port")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PortId")
                        .HasColumnType("int");

                    b.Property<int?>("Positive")
                        .HasColumnType("int");

                    b.Property<int>("QId")
                        .HasColumnType("int");

                    b.Property<string>("RegisterName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("SourceId")
                        .HasColumnType("int");

                    b.Property<byte?>("Status")
                        .HasColumnType("tinyint");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("VesselId")
                        .HasColumnType("int");

                    b.Property<string>("VesselName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("VetDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("VetGUI")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("VetTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("VettingCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VetId");

                    b.HasIndex("InspectionTypeId");

                    b.ToTable("VettingInfo");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");

                    b.HasData(
                        new
                        {
                            Id = "7d2d85b3-4131-406e-9cb1-c03c59641efb",
                            ConcurrencyStamp = "236a71a9-7975-442f-b952-24c317b2c1dc",
                            Name = "Admin",
                            NormalizedName = "Admin"
                        },
                        new
                        {
                            Id = "ac00f56c-c9d7-401d-91af-d04ae9e6433d",
                            ConcurrencyStamp = "f9ba8b32-af76-47c1-8f4b-e8c1dc56c54d",
                            Name = "Manager",
                            NormalizedName = "Manager"
                        },
                        new
                        {
                            Id = "c9c87fb6-80ad-41da-9963-ce3a5cf6ab67",
                            ConcurrencyStamp = "efd1e724-75de-460f-b0ae-1b6df994411c",
                            Name = "User",
                            NormalizedName = "User"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Attendance.Models.Briefcase", b =>
                {
                    b.HasOne("Attendance.Models.InspectionTypes", "InspectionType")
                        .WithMany()
                        .HasForeignKey("InspectionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Attendance.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Attendance.Models.Vessel", "Vessel")
                        .WithMany()
                        .HasForeignKey("VesselId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Attendance.Models.BriefcaseQuestionnaire", b =>
                {
                    b.HasOne("Attendance.Models.Briefcase", "Briefcase")
                        .WithMany("Questionnaires")
                        .HasForeignKey("BriefcaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Attendance.Models.VIQInfoModel", "Questionnaire")
                        .WithMany()
                        .HasForeignKey("QId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Attendance.Models.QuestionPool", b =>
                {
                    b.HasOne("Attendance.Models.QuestionPool", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_QuestionPool_Parent")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Attendance.Models.UserQuestionnaire", b =>
                {
                    b.HasOne("Attendance.Models.VIQInfoModel", "VIQInfo")
                        .WithMany("UserQuestionnaires")
                        .HasForeignKey("QId")
                        .HasConstraintName("FK_UserQuestionnaire_Questionnaires")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Attendance.Models.VettingAttachment", b =>
                {
                    b.HasOne("Attendance.Models.Vetting", "Vetting")
                        .WithMany("VettingAttachments")
                        .HasForeignKey("VETId", "ObjectId")
                        .HasConstraintName("FK_VettingAttachment_Vetting")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Attendance.Models.VettingInfo", b =>
                {
                    b.HasOne("Attendance.Models.InspectionTypes", "InspectionType")
                        .WithMany()
                        .HasForeignKey("InspectionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Attendance.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Attendance.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Attendance.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Attendance.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
