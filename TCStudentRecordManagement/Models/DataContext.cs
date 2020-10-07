using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TCStudentRecordManagement.Models
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        // DbSets for each table
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cohort> Cohorts { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<AssignmentType> AssignmentTypes { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<Timesheet> Timesheets { get; set; }

        public virtual DbSet<Attendance> AttendanceRecords { get; set; }
        public virtual DbSet<AttendanceState> AttendanceStates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Users

            modelBuilder.Entity<User>(entity =>
            {

                // Sample data
                List<User> sampleUserData = new List<User>() {
                    new User { UserID = 1, Firstname = "John", Lastname = "Dough", Email = "john.dough@abc.com", Rights = 1},
                    new User { UserID = 2, Firstname = "Anne", Lastname = "Jackson", Email = "anne.jackson@abc.com", Rights = 1},
                    new User { UserID = 3, Firstname = "Ivan", Lastname = "Patterson", Email = "ivan.patterson@abc.com", Rights = 1},
                    new User { UserID = 4,  Firstname = "Trevor", Lastname = "Noah", Email = "trevor.noah@abc.com", Rights = 1},
                    new User { UserID = 5, Firstname = "Peter", Lastname = "Patsie", Email = "peter.patsie@abc.com", Rights = 1},
                    new User { UserID = 6, Firstname = "Jane", Lastname = "Smitherson", Email = "jane.smitherson@abc.com", Rights = 2 }
                };

                sampleUserData.ForEach(x => entity.HasData(x));

            });


            // Cohorts

            modelBuilder.Entity<Cohort>(entity =>
            {

                // Sample data
                entity.HasData(new Cohort { CohortID = -9999, Name = "Common", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue });
                entity.HasData(new Cohort { CohortID = -1, Name = "4.1 Summer/Autumn 2020", StartDate = new DateTime(2020, 6, 15), EndDate = new DateTime(2020, 10, 30) });
                entity.HasData(new Cohort { CohortID = -2, Name = "4.2 Autumn/Winter 2020", StartDate = new DateTime(2020, 8, 15), EndDate = new DateTime(2020, 12, 31) });
            });

            // Students

            modelBuilder.Entity<Student>(entity =>
            {
                // Foreign keys in the table
                entity.HasIndex(x => x.UserID).HasName($"FK_{nameof(Student)}_{nameof(User)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Student)}_{nameof(Cohort)}");

                // Define relationship for UserID and Student
                //entity.HasOne(student => student.UserData).;

                // Define a relationship between Student and Cohort
                entity.HasOne(student => student.CohortMember)
                .WithMany(cohort => cohort.Students)
                .HasForeignKey(student => student.CohortID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Student)}_{nameof(Cohort)}");

                List<Student> sampleStudentData = new List<Student>()
                {
                    new Student { StudentID = -1, CohortID = -1, UserID = 1},
                    new Student { StudentID = -2, CohortID = -1, UserID = 2},
                    new Student { StudentID = -3, CohortID = -1, UserID = 3},
                    new Student { StudentID = -4, CohortID = -2, UserID = 4},
                    new Student { StudentID = -5, CohortID = -2, UserID = 5}
                };

                sampleStudentData.ForEach(x => entity.HasData(x));

            });

            // AssignmentTypes

            modelBuilder.Entity<AssignmentType>(entity =>
            {
                List<AssignmentType> sampleAssignmentTypes = new List<AssignmentType>()
                {
                    new AssignmentType { TypeID = -1, Description = "Classroom lectures"},
                    new AssignmentType { TypeID = -2, Description = "Online self-study"},
                    new AssignmentType { TypeID = -3, Description = "Break"},
                    new AssignmentType { TypeID = -4, Description = "Practice" },
                    new AssignmentType { TypeID = -5, Description = "Weekend assignment" },
                    new AssignmentType { TypeID = -6, Description = "Milestone" },
                    new AssignmentType { TypeID = -7, Description = "Capstone" }
                };

                sampleAssignmentTypes.ForEach(x => entity.HasData(x));

            });

            // Topics

            modelBuilder.Entity<Topic>(entity =>
            {
                List<Topic> sampleTopics = new List<Topic>()
                {
                    new Topic { TopicID = -1, Description = "Generic" },
                    new Topic { TopicID = -2, Description = "C# fundamentals" },
                    new Topic { TopicID = -3, Description = "HTML5 and CSS" },
                    new Topic { TopicID = -4, Description = "Javascript fundamentals" },
                    new Topic { TopicID = -5, Description = "Javascript AJAX and API" },
                    new Topic { TopicID = -6, Description = "React fundamentals" },
                    new Topic { TopicID = -7, Description = "React/Redux" },
                    new Topic { TopicID = -8, Description = "SQL fundamentals" },
                    new Topic { TopicID = -9, Description = "Wordpress" },
                    new Topic { TopicID = -10, Description = "PHP fundamentals" },
                    new Topic { TopicID = -11, Description = "PHP APIs" },
                    new Topic { TopicID = -12, Description = "C# OOP" },
                    new Topic { TopicID = -13, Description = "C# Entity Framework" },
                    new Topic { TopicID = -14, Description = "C# MVC" },
                    new Topic { TopicID = -15, Description = "C# WebAPI" },
                    new Topic { TopicID = -16, Description = "C# WebAPI and React" }
                };

                sampleTopics.ForEach(x => entity.HasData(x));

            });

            // Assignments

            modelBuilder.Entity<Assignment>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.TopicID).HasName($"FK_{nameof(Assignment)}_{nameof(Topic)}");
                entity.HasIndex(x => x.TypeID).HasName($"FK_{nameof(Assignment)}_{nameof(AssignmentType)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Assignment)}_{nameof(Cohort)}");

                // Relationships
                entity.HasOne(assignment => assignment.AssignmentTopic)
                .WithMany(topic => topic.Assignments)
                .HasForeignKey(assignment => assignment.TopicID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Assignment)}_{nameof(Topic)}");

                entity.HasOne(assignment => assignment.Type)
                .WithMany(type => type.Assignments)
                .HasForeignKey(assignment => assignment.TypeID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Assignment)}_{nameof(AssignmentType)}");

                entity.HasOne(assignment => assignment.AssignmentCohort)
                .WithMany(cohort => cohort.Assignments)
                .HasForeignKey(assignment => assignment.CohortID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName($"FK_{nameof(Assignment)}_{nameof(Cohort)}");

                // Sample data

                List<Assignment> sampleAssignments = new List<Assignment>()
                {
                    new Assignment { AssignmentID = -1, CohortID = -9999, TypeID = -1, TopicID = -1, Title = "Classroom instruction", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue, DocURL = null },
                    new Assignment { AssignmentID = -2, CohortID = -9999, TypeID = -2, TopicID = -1, Title = "Online self-study", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue, DocURL = null },
                    new Assignment { AssignmentID = -3, CohortID = -9999, TypeID = -3, TopicID = -1, Title = "Break", StartDate = DateTime.MinValue, EndDate = DateTime.MaxValue, DocURL = null },
                    new Assignment { AssignmentID = -4, CohortID = -1, TypeID = -5, TopicID = -2, Title = "Tic-Tac-Toe", StartDate = new DateTime(2020, 6, 25), EndDate = new DateTime(2020, 6, 28), DocURL = "http://does-not-exist.com/" },
                    new Assignment { AssignmentID = -5, CohortID = -1, TypeID = -4, TopicID = -13, Title = "Entity Framework Practice 2: Cars", StartDate = new DateTime(2020, 9, 21), EndDate = new DateTime(2020, 9, 22), DocURL = "http://does-not-exist.com/" },
                    new Assignment { AssignmentID = -6, CohortID = -2, TypeID = -6, TopicID = -7, Title = "React/Redux Group Milestone 2", StartDate = new DateTime(2020, 9, 21), EndDate = new DateTime(2020, 9, 22), DocURL = "http://does-not-exist.com/" },
                    new Assignment { AssignmentID = -7, CohortID = -2, TypeID = -6, TopicID = -15, Title = "Personal Portfolio: Milestone 1", StartDate = new DateTime(2020, 7, 15), EndDate = new DateTime(2020, 7, 22), DocURL = "http://does-not-exist.com/" },
                    new Assignment { AssignmentID = -8, CohortID = -2, TypeID = -5, TopicID = -4, Title = "Javascript Todo App", StartDate = new DateTime(2020, 7, 21), EndDate = new DateTime(2020, 9, 23), DocURL = "http://does-not-exist.com/" }
                };

                sampleAssignments.ForEach(x => entity.HasData(x));

            });


            // Timesheets

            modelBuilder.Entity<Timesheet>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.StudentID).HasName($"FK_{nameof(Timesheet)}_{nameof(Student)}");
                entity.HasIndex(x => x.AssignmentID).HasName($"FK_{nameof(Timesheet)}_{nameof(Assignment)}");

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
                .HasConstraintName($"FK_{nameof(Timesheet)}_{nameof(Assignment)}");

                // Will be possible in EF Core 5:
                // Ref: https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties?tabs=fluent-api%2Cwithout-nrt#precision-and-scale
                //entity.Property(timesheet => timesheet.TimeAllocation).HasPrecision(3, 2);

                // Sample data
                List<Timesheet> sampleTimesheets = new List<Timesheet>()
                {
                    new Timesheet { RecordID = -1, StudentID = -1, AssignmentID = -1, Date = new DateTime(2020,6,25), TimeAllocation = (decimal)8 },
                    new Timesheet { RecordID = -2, StudentID = -1, AssignmentID = -3, Date = new DateTime(2020,6,25), TimeAllocation = (decimal)1 },
                    new Timesheet { RecordID = -3, StudentID = -1, AssignmentID = -4, Date = new DateTime(2020,6,25), TimeAllocation = (decimal)1.5 },
                    new Timesheet { RecordID = -4, StudentID = -1, AssignmentID = -4, Date = new DateTime(2020,6,26), TimeAllocation = (decimal)1.0 },
                    new Timesheet { RecordID = -5, StudentID = -4, AssignmentID = -8, Date = new DateTime(2020,7,22), TimeAllocation = (decimal)2.25 },
                    new Timesheet { RecordID = -6, StudentID = -4, AssignmentID = -5, Date = new DateTime(2020,9,21), TimeAllocation = (decimal)5.75 },
                    new Timesheet { RecordID = -7, StudentID = -4, AssignmentID = -6, Date = new DateTime(2020,9,21), TimeAllocation = (decimal)4.5 },
                    new Timesheet { RecordID = -8, StudentID = -4, AssignmentID = -7, Date = new DateTime(2020,7,15), TimeAllocation = (decimal)3.25 },
                    new Timesheet { RecordID = -9, StudentID = -4, AssignmentID = -7, Date = new DateTime(2020,7,16), TimeAllocation = (decimal)1.0 }
                };
     
            });

            // AttendanceStateID

            modelBuilder.Entity<AttendanceState>(entity =>
            {
                // Sample data

                List<AttendanceState> sampleAttendanceStates = new List<AttendanceState>()
                {
                    new AttendanceState { StateID = -1, Description = "Present" },
                    new AttendanceState { StateID = -2, Description = "Absent with notice" },
                    new AttendanceState { StateID = -3, Description = "Absent without notice" }
                };

                sampleAttendanceStates.ForEach(x => entity.HasData(x));
            });

            // Attendance

            modelBuilder.Entity<Attendance>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.AttendanceStateID).HasName($"FK_{nameof(Attendance)}_{nameof(AttendanceState)}");
                entity.HasIndex(x => x.StaffID).HasName($"FK_{nameof(Attendance)}_{nameof(User)}");
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

                // Sample data

                List<Attendance> sampleAttendanceData = new List<Attendance>()
                {
                    new Attendance { RecordID = -1, StudentID = -1, AttendanceStateID = -1, Date = new DateTime(2020,6,25), StaffID = 6 },
                    new Attendance { RecordID = -2, StudentID = -4, AttendanceStateID = -2, Date = new DateTime(2020,6,25), StaffID = 6 }
                };

                sampleAttendanceData.ForEach(x => entity.HasData(x));

            });

            // Notices

            modelBuilder.Entity<Notice>(entity =>
            {
                // Foreign keys
                entity.HasIndex(x => x.StaffID).HasName($"FK_{nameof(Notice)}_{nameof(User)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Notice)}_{nameof(Cohort)}");

                // Relationships
                entity.HasOne(notice => notice.ForCohort)
                .WithMany(cohorts => cohorts.Notices)
                .HasForeignKey(notice => notice.CohortID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Notice)}_{nameof(Cohort)}");

                entity.HasOne(notice => notice.Staff)
                .WithMany(user => user.Notices)
                .HasForeignKey(Notice => Notice.StaffID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName($"FK_{nameof(Notice)}_{nameof(User)}");

                // Sample data

                List<Notice> sampleNoticeData = new List<Notice>()
                {
                    new Notice { NoticeID= -1, CohortID = -1, ValidFrom = new DateTime(2020,6,25), StaffID = 6  },
                    new Notice { NoticeID= -2, CohortID = -2, ValidFrom = new DateTime(2020,6,25), StaffID = 6  }
                };

                sampleNoticeData.ForEach(x => entity.HasData(x));

            });

        } // End of OnModelCreating
    }

}
