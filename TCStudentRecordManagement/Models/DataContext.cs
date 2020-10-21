using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace TCStudentRecordManagement.Models
{
    public class DataContext: DbContext
    {

        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(APIConfig.Configuration["sqldb:ConnectionString"]);
            }
            
            optionsBuilder.EnableSensitiveDataLogging();

            // Disable logging of EF initialization
            // Ref: https://stackoverflow.com/a/56424676/12802214
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ContextInitialized));

        }

        // DbSets for each table
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Cohort> Cohorts { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<Timesheet> Timesheets { get; set; }

        public virtual DbSet<Attendance> AttendanceRecords { get; set; }
        public virtual DbSet<AttendanceState> AttendanceStates { get; set; }

        public virtual DbSet<Notice> Notices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Users

            

            // Staff

            modelBuilder.Entity<Staff>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.UserID).HasName($"FK_{nameof(Staff)}_{nameof(User)}");

                // Relationships
                entity.HasOne(staff => staff.UserData)
                .WithOne(user => user.StaffData)
                .HasConstraintName($"FK_{nameof(Staff)}_{nameof(User)}");


            });

            // Cohorts - Sample data generated through external SQL script


            // Students

            modelBuilder.Entity<Student>(entity =>
            {
                // Foreign keys in the table
                entity.HasIndex(x => x.UserID).HasName($"FK_{nameof(Student)}_{nameof(User)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Student)}_{nameof(Cohort)}");

                // Relationships
                entity.HasOne(student => student.UserData)
                .WithOne(user => user.StudentData)
                .HasConstraintName($"FK_{nameof(Student)}_{nameof(User)}");

                entity.HasOne(student => student.CohortMember)
                .WithMany(cohort => cohort.Students)
                .HasForeignKey(student => student.CohortID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Student)}_{nameof(Cohort)}");


            });

            // TaskType - Sample data generated through external SQL script

            // Units - Sample data generated through external SQL script

            // Tasks

            modelBuilder.Entity<Task>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.UnitID).HasName($"FK_{nameof(Task)}_{nameof(Unit)}");
                entity.HasIndex(x => x.TypeID).HasName($"FK_{nameof(Task)}_{nameof(TaskType)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Task)}_{nameof(Cohort)}");

                // Relationships
                entity.HasOne(assignment => assignment.FromUnit)
                .WithMany(unit => unit.Tasks)
                .HasForeignKey(assignment => assignment.UnitID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Task)}_{nameof(Unit)}");

                entity.HasOne(assignment => assignment.Type)
                .WithMany(type => type.Tasks)
                .HasForeignKey(assignment => assignment.TypeID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Task)}_{nameof(TaskType)}");

                entity.HasOne(assignment => assignment.AssignedCohort)
                .WithMany(cohort => cohort.Tasks)
                .HasForeignKey(assignment => assignment.CohortID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName($"FK_{nameof(Task)}_{nameof(Cohort)}");

            });


            // Timesheets

            modelBuilder.Entity<Timesheet>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.StudentID).HasName($"FK_{nameof(Timesheet)}_{nameof(Student)}");
                entity.HasIndex(x => x.AssignmentID).HasName($"FK_{nameof(Timesheet)}_{nameof(Task)}");

                // Relationships
                entity.HasOne(timesheet => timesheet.ForStudent)
                .WithMany(student => student.Timesheets)
                .HasForeignKey(timesheet => timesheet.StudentID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Timesheet)}_{nameof(Student)}");

                entity.HasOne(timesheet => timesheet.AssignmentAlloc)
                .WithMany(assignment => assignment.Timesheets)
                .HasForeignKey(timesheet => timesheet.AssignmentID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Timesheet)}_{nameof(Task)}");

                // Will be possible in EF Core 5:
                // Ref: https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=fluent-api%2Cwithout-nrt#precision-and-scale
                //entity.Property(timesheet => timesheet.TimeAllocation).HasPrecision(3, 2);

     
            });

            // AttendanceStateID - Sample data generated through external SQL script
                
            // Attendance

            modelBuilder.Entity<Attendance>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.AttendanceStateID).HasName($"FK_{nameof(Attendance)}_{nameof(AttendanceState)}");
                entity.HasIndex(x => x.StaffID).HasName($"FK_{nameof(Attendance)}_{nameof(Staff)}");
                entity.HasIndex(x => x.StudentID).HasName($"FK_{nameof(Attendance)}_{nameof(Student)}");

                // Relationships
                entity.HasOne(attendance => attendance.AttendanceType)
                .WithMany(attendancetype => attendancetype.AttendancesOfType)
                .HasForeignKey(attendance => attendance.AttendanceStateID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Attendance)}_{nameof(AttendanceState)}");

                entity.HasOne(attendance => attendance.StudentDetails)
                .WithMany(students => students.AttendanceRecord)
                .HasForeignKey(attendance => attendance.StudentID)
                .OnDelete(DeleteBehavior.Restrict) 
                .HasConstraintName($"FK_{nameof(Attendance)}_{nameof(Student)}");

                entity.HasOne(attendance => attendance.RecordedBy)
                .WithMany(staff => staff.AttendanceTaken)
                .HasForeignKey(attendance => attendance.StaffID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Attendance)}_{nameof(Staff)}");

            });

            // Notices

            modelBuilder.Entity<Notice>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.StaffID).HasName($"FK_{nameof(Notice)}_{nameof(Staff)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Notice)}_{nameof(Cohort)}");

                // Relationships
                entity.HasOne(notice => notice.ForCohort)
                .WithMany(cohorts => cohorts.Notices)
                .HasForeignKey(notice => notice.CohortID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Notice)}_{nameof(Cohort)}");

                entity.HasOne(notice => notice.ByStaff)
                .WithMany(staff => staff.Notices)
                .HasForeignKey(Notice => Notice.StaffID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Notice)}_{nameof(Staff)}");

            });

        } // End of OnModelCreating
    }

}
