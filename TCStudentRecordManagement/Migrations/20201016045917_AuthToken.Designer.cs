﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TCStudentRecordManagement.Models;

namespace TCStudentRecordManagement.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20201016045917_AuthToken")]
    partial class AuthToken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TCStudentRecordManagement.Models.Attendance", b =>
                {
                    b.Property<int>("RecordID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RecordID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AttendanceStateID")
                        .HasColumnName("AttendanceStateID")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnName("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnName("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("StaffID")
                        .HasColumnName("StaffID")
                        .HasColumnType("int");

                    b.Property<int>("StudentID")
                        .HasColumnName("StudentID")
                        .HasColumnType("int");

                    b.HasKey("RecordID");

                    b.HasIndex("AttendanceStateID")
                        .HasName("FK_Attendance_AttendanceState");

                    b.HasIndex("StaffID")
                        .HasName("FK_Attendance_Staff");

                    b.HasIndex("StudentID")
                        .HasName("FK_Attendance_Student");

                    b.ToTable("attendance");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.AttendanceState", b =>
                {
                    b.Property<int>("StateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StateID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasColumnType("varchar(50)");

                    b.HasKey("StateID");

                    b.ToTable("attendance_states");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Cohort", b =>
                {
                    b.Property<int>("CohortID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("CohortID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate")
                        .HasColumnName("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("Name")
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("CohortID");

                    b.ToTable("cohorts");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Notice", b =>
                {
                    b.Property<int>("NoticeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NoticeID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CohortID")
                        .HasColumnName("CohortID")
                        .HasColumnType("int");

                    b.Property<string>("HTML")
                        .HasColumnName("HTML")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Markdown")
                        .HasColumnName("Markdown")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StaffID")
                        .HasColumnName("StaffID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnName("ValidFrom")
                        .HasColumnType("datetime2");

                    b.HasKey("NoticeID");

                    b.HasIndex("CohortID")
                        .HasName("FK_Notice_Cohort");

                    b.HasIndex("StaffID")
                        .HasName("FK_Notice_Staff");

                    b.ToTable("notices");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Staff", b =>
                {
                    b.Property<int>("StaffID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StaffID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("SuperUser")
                        .HasColumnName("SuperUser")
                        .HasColumnType("bit");

                    b.Property<int>("UserID")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.HasKey("StaffID");

                    b.HasIndex("UserID")
                        .IsUnique()
                        .HasName("FK_Staff_User");

                    b.ToTable("staff");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Student", b =>
                {
                    b.Property<int>("StudentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("StudentID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BearTracksID")
                        .HasColumnName("BearTracksID")
                        .HasColumnType("varchar(10)");

                    b.Property<int>("CohortID")
                        .HasColumnName("CohortID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.HasKey("StudentID");

                    b.HasIndex("CohortID")
                        .HasName("FK_Student_Cohort");

                    b.HasIndex("UserID")
                        .IsUnique()
                        .HasName("FK_Student_User");

                    b.ToTable("students");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Task", b =>
                {
                    b.Property<int>("TaskID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TaskID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CohortID")
                        .HasColumnName("CohortID")
                        .HasColumnType("int");

                    b.Property<string>("DocURL")
                        .HasColumnName("DocURL")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnName("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnName("Title")
                        .HasColumnType("varchar(100)");

                    b.Property<int>("TypeID")
                        .HasColumnName("TypeID")
                        .HasColumnType("int");

                    b.Property<int>("UnitID")
                        .HasColumnName("UnitID")
                        .HasColumnType("int");

                    b.HasKey("TaskID");

                    b.HasIndex("CohortID")
                        .HasName("FK_Task_Cohort");

                    b.HasIndex("TypeID")
                        .HasName("FK_Task_TaskType");

                    b.HasIndex("UnitID")
                        .HasName("FK_Task_Unit");

                    b.ToTable("tasks");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.TaskType", b =>
                {
                    b.Property<int>("TypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("TypeID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasColumnType("varchar(50)");

                    b.HasKey("TypeID");

                    b.ToTable("task_types");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Timesheet", b =>
                {
                    b.Property<int>("RecordID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("RecordID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AssignmentID")
                        .HasColumnName("AssignmentID")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnName("Date")
                        .HasColumnType("date");

                    b.Property<int>("StudentID")
                        .HasColumnName("StudentID")
                        .HasColumnType("int");

                    b.Property<decimal>("TimeAllocation")
                        .HasColumnName("TimeAllocation")
                        .HasColumnType("decimal(3,2)");

                    b.HasKey("RecordID");

                    b.HasIndex("AssignmentID")
                        .HasName("FK_Timesheet_Task");

                    b.HasIndex("StudentID")
                        .HasName("FK_Timesheet_Student");

                    b.ToTable("timesheets");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Unit", b =>
                {
                    b.Property<int>("UnitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UnitID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("Description")
                        .HasColumnType("varchar(50)");

                    b.HasKey("UnitID");

                    b.ToTable("units");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UserID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active")
                        .HasColumnName("Active")
                        .HasColumnType("bit");

                    b.Property<string>("ActiveToken")
                        .HasColumnName("ActiveToken")
                        .HasColumnType("varchar(2048)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("Email")
                        .HasColumnType("varchar(320)");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnName("Firstname")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasColumnName("Lastname")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserID");

                    b.ToTable("users");
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Attendance", b =>
                {
                    b.HasOne("TCStudentRecordManagement.Models.AttendanceState", "AttendanceType")
                        .WithMany("AttendancesOfType")
                        .HasForeignKey("AttendanceStateID")
                        .HasConstraintName("FK_Attendance_AttendanceState")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.Staff", "RecordedBy")
                        .WithMany("AttendanceTaken")
                        .HasForeignKey("StaffID")
                        .HasConstraintName("FK_Attendance_Staff")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.Student", "StudentDetails")
                        .WithMany("AttendanceRecord")
                        .HasForeignKey("StudentID")
                        .HasConstraintName("FK_Attendance_Student")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Notice", b =>
                {
                    b.HasOne("TCStudentRecordManagement.Models.Cohort", "ForCohort")
                        .WithMany("Notices")
                        .HasForeignKey("CohortID")
                        .HasConstraintName("FK_Notice_Cohort")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.Staff", "ByStaff")
                        .WithMany("Notices")
                        .HasForeignKey("StaffID")
                        .HasConstraintName("FK_Notice_Staff")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Staff", b =>
                {
                    b.HasOne("TCStudentRecordManagement.Models.User", "UserData")
                        .WithOne("StaffData")
                        .HasForeignKey("TCStudentRecordManagement.Models.Staff", "UserID")
                        .HasConstraintName("FK_Staff_User")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Student", b =>
                {
                    b.HasOne("TCStudentRecordManagement.Models.Cohort", "CohortMember")
                        .WithMany("Students")
                        .HasForeignKey("CohortID")
                        .HasConstraintName("FK_Student_Cohort")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.User", "UserData")
                        .WithOne("StudentData")
                        .HasForeignKey("TCStudentRecordManagement.Models.Student", "UserID")
                        .HasConstraintName("FK_Student_User")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Task", b =>
                {
                    b.HasOne("TCStudentRecordManagement.Models.Cohort", "AssignedCohort")
                        .WithMany("Tasks")
                        .HasForeignKey("CohortID")
                        .HasConstraintName("FK_Task_Cohort")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.TaskType", "Type")
                        .WithMany("Tasks")
                        .HasForeignKey("TypeID")
                        .HasConstraintName("FK_Task_TaskType")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.Unit", "FromUnit")
                        .WithMany("Tasks")
                        .HasForeignKey("UnitID")
                        .HasConstraintName("FK_Task_Unit")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("TCStudentRecordManagement.Models.Timesheet", b =>
                {
                    b.HasOne("TCStudentRecordManagement.Models.Task", "AssignmentAlloc")
                        .WithMany("Timesheets")
                        .HasForeignKey("AssignmentID")
                        .HasConstraintName("FK_Timesheet_Task")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TCStudentRecordManagement.Models.Student", "ForStudent")
                        .WithMany("Timesheets")
                        .HasForeignKey("StudentID")
                        .HasConstraintName("FK_Timesheet_Student")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
