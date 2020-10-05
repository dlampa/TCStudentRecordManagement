using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            modelBuilder.Entity<Cohort>(entity =>
            {


            });


            modelBuilder.Entity<User>(entity =>
            {
                

                // entity.HasData(new { ID = 1, Name = "Test User", Email = "test@abc.com" });
            });

        }
    }
    
}
