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

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cohort> Cohorts { get; set; }
        public virtual DbSet<Student> Students { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

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

            modelBuilder.Entity<Cohort>(entity =>
            {

                // Sample data
                entity.HasData(new Cohort { CohortID = -1, Name = "4.1 Summer/Autumn 2020", StartDate = new DateTime(2020, 6, 15), EndDate = new DateTime(2020, 10, 30) });
                entity.HasData(new Cohort { CohortID = -2, Name = "4.2 Autumn/Winter 2020", StartDate = new DateTime(2020, 8, 15), EndDate = new DateTime(2020, 12, 31) });
            });

            modelBuilder.Entity<Student>(entity =>
            {
                // Foreign keys in the table
                entity.HasIndex(x => x.UserID).HasName($"FK_{nameof(Student)}_{nameof(User)}");
                entity.HasIndex(x => x.CohortID).HasName($"FK_{nameof(Student)}_{nameof(Cohort)}");

                // Define relationship for UserID and Student
                //entity.HasOne(student => student.UserData).;

                // Define a relationship between Student and Cohort
                entity.HasOne(student => student.CohortMember)
                .WithMany(cohorts => cohorts.Students)
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




        }
    }

}
