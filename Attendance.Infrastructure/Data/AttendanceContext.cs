using Attendance.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;

namespace Attendance.Infrastructure.Data
{
    public class AttendanceContext : IdentityDbContext<User>
    {
        public AttendanceContext(DbContextOptions<AttendanceContext> options) : base(options)
        { }
        public DbSet<QuestionPool> QuestionPoolNew { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<VIQInfoModel> VIQInfo { get; set; }
        public DbSet<VIQ> VIQ { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<InspectionTypes> InspectionTypes { get; set; }
        public DbSet<VettingInfo> VettingInfo { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<VettingAttachment> VettingAttachment { get; set; }
        public DbSet<Vetting> Vetting { get; set; }
        public DbSet<Vessel> Vessel { get; set; }
        public DbSet<UserQuestionnaire> UserQuestionnaire { get; set; }
        public DbSet<InspectionSource> InspectionSource { get; set; }
        public DbSet<Briefcase> Briefcase { get; set; }
        public DbSet<BriefcaseQuestionnaire> BriefcaseQuestionnaires { get; set; }
        public DbSet<FormDetail> FormDetail { get; set; }
        public DbSet<CrewRank> CrewRank { get; set; }
        public DbSet<ObservationsSire2> ObservationsSire2 { get; set; }
        public DbSet<ObservationsSire2SOC> ObservationsSire2SOC { get; set; }
        public DbSet<ObservationsSire2NOC> ObservationsSire2NOC { get; set; }

        public DbSet<ObservationsSire2Assignees> ObservationsSire2Assignees { get; set; }
        public DbSet<ObservationsSire2Attachments> ObservationsSire2Attachments { get; set; }
        public DbSet<CWCREW> CWCREW { get; set; }
        public DbSet<Ranks> Ranks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vetting>().HasKey(e => new { e.VETId, e.ObjectId });
            // modelBuilder.Entity<User>().HasKey(x => x.UserId);
            modelBuilder.Entity<VIQ>().HasNoKey();
            //modelBuilder.Entity<User_Roles>().HasNoKey();
            modelBuilder.Entity<QuestionPool>(b =>
            {
                b.HasKey(x => x.questionid);
                b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).HasConstraintName("FK_QuestionPool_Parent").OnDelete(DeleteBehavior.NoAction).IsRequired(false);
            });
            modelBuilder.Entity<VettingAttachment>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne(x => x.Vetting).WithMany(x => x.VettingAttachments).HasForeignKey(x => new { x.VETId, x.ObjectId }).HasConstraintName("FK_VettingAttachment_Vetting").OnDelete(DeleteBehavior.Cascade).IsRequired();
            });
            modelBuilder.Entity<Briefcase>().HasMany(_ => _.Questionnaires).WithOne(x => x.Briefcase).HasForeignKey(x => x.BriefcaseId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //modelBuilder.Entity<BriefcaseQuestionnaire>().HasOne(_ => _.Briefcase).WithMany(_ => BriefcaseQuestionnaires).HasForeignKey(x => x.BriefcaseId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<BriefcaseQuestionnaire>().HasIndex(x => new { x.QId, x.BriefcaseId }).IsUnique();
            modelBuilder.Entity<Briefcase>().HasIndex(x => new { x.UserId, x.InspectionCode }).IsUnique();
            modelBuilder.Entity<Major>().ToView(nameof(Majors)).HasKey(_ => _.MajorId);
            modelBuilder.Entity<UserQuestionnaire>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne(x => x.VIQInfo).WithMany(x => x.UserQuestionnaires).HasForeignKey(x => x.QId).HasConstraintName("FK_UserQuestionnaire_Questionnaires").OnDelete(DeleteBehavior.NoAction).IsRequired();
                b.HasIndex(x => new { x.UserId, x.QId }).IsUnique().HasName("IX_UserQuestionnaire_User_Q");
            });
            modelBuilder.Entity<InspectionSource>().ToView(nameof(InspectionSource)).HasKey(_ => _.InspectionSourceId);
            modelBuilder.Entity<IdentityRole>().HasData(new List<IdentityRole>() {
                new IdentityRole {
                    Id = "7d2d85b3-4131-406e-9cb1-c03c59641efb",
                    Name = "Admin",
                    NormalizedName = "Admin",
                    ConcurrencyStamp = "236a71a9-7975-442f-b952-24c317b2c1dc"
                },
                new IdentityRole {
                    Id ="ac00f56c-c9d7-401d-91af-d04ae9e6433d",
                    Name = "Manager",
                    NormalizedName = "Manager",
                    ConcurrencyStamp = "f9ba8b32-af76-47c1-8f4b-e8c1dc56c54d"
                },
                new IdentityRole {
                    Id = "c9c87fb6-80ad-41da-9963-ce3a5cf6ab67",
                    Name = "User",
                    NormalizedName = "User",
                    ConcurrencyStamp = "efd1e724-75de-460f-b0ae-1b6df994411c"
                }
            });

            modelBuilder.Entity<FormDetail>().HasNoKey();
            modelBuilder.Entity<ObservationsSire2>(b =>
            {
                b.HasKey(x => x.id);
            });

            modelBuilder.Entity<ObservationsSire2Assignees>(b =>
            {
                b.HasKey(x => x.id);
            });

            modelBuilder.Entity<ObservationsSire2Attachments>(b =>
            {
                b.HasKey(x => x.id);
            });
            modelBuilder.Entity<CWCREW>(b =>
            {
                b.HasKey(x => x.id);
            });
            modelBuilder.Entity<Ranks>(b =>
            {
                b.HasKey(x => x.id);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}