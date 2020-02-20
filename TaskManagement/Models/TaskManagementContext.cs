﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace TaskManagement.Models
{
    public partial class TaskManagementContext : DbContext
    {
        public TaskManagementContext()
        {
        }

        public IConfiguration Configuration { get; }

        public TaskManagementContext(DbContextOptions<TaskManagementContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }
        /*This code creates a DbSetproperty for each entity set.
         * In Entity Framework terminology, an entity set typically
         * corresponds to a database table, and an entity corresponds
         * to a row in the table.*/
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<JAccountCompany> JAccountCompany { get; set; }
        public virtual DbSet<JAccountTask> JAccountTask { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskState> TaskState { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Management"));
            }
        }
        //Method OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.AccountCreatedDateTime)
                    .HasColumnName("Account_Created_DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.AccountEmail)
                    .IsRequired()
                    .HasColumnName("Account_Email")
                    .HasMaxLength(120);

                entity.Property(e => e.AccountPassword)
                    .IsRequired()
                    .HasColumnName("Account_Password");

                entity.Property(e => e.AccountRoleId).HasColumnName("Account_Role_ID");

                entity.Property(e => e.AccountRoleLastChangeDateTime)
                    .HasColumnName("Account_Role_LastChange_DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.AccountUserBirthDate)
                    .HasColumnName("Account_User_BirthDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.AccountUserFirstName)
                    .HasColumnName("Account_User_FirstName")
                    .HasMaxLength(80);

                entity.Property(e => e.AccountUserLastName)
                    .HasColumnName("Account_User_LastName")
                    .HasMaxLength(80);

                entity.Property(e => e.AccountImageLink)
                    .HasColumnName("Account_Image_Link")
                    .HasMaxLength(255);

                entity.HasOne(d => d.AccountRole)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.AccountRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Account__Account__32E0915F");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("Comment_ID");

                entity.Property(e => e.CommentAccountId).HasColumnName("Comment_Account_ID");

                entity.Property(e => e.CommentProjectId).HasColumnName("Comment_Project_ID");

                entity.Property(e => e.CommentTaskId).HasColumnName("Comment_Task_ID");

                entity.Property(e => e.CommentText)
                    .HasColumnName("Comment_Text")
                    .HasMaxLength(4000);

                entity.HasOne(d => d.CommentAccount)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CommentAccountId)
                    .HasConstraintName("FK__Comment__Comment__33D4B598");

                entity.HasOne(d => d.CommentProject)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CommentProjectId)
                    .HasConstraintName("FK__Comment__Comment__34C8D9D1");

                entity.HasOne(d => d.CommentTask)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CommentTaskId)
                    .HasConstraintName("FK__Comment__Comment__35BCFE0A");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId).HasColumnName("Company_ID");

                entity.Property(e => e.CompanyCreatedDateTime)
                    .HasColumnName("Company_Created_DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.CompanyInfo)
                    .HasColumnName("Company_Info")
                    .HasMaxLength(400);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnName("Company_Name")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<JAccountCompany>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.CompanyId });

                entity.ToTable("J_AccountCompany");

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.CompanyId).HasColumnName("Company_ID");

                entity.Property(e => e.CompanyCreator).HasColumnName("Company_Creator");

                entity.Property(e => e.CompanyOwner).HasColumnName("Company_Owner");

                entity.Property(e => e.UserAssignedDateTime)
                    .HasColumnName("User_Assigned_DateTime")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.JAccountCompany)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__J_Account__Accou__36B12243");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.JAccountCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__J_Account__Compa__37A5467C");
            });

            modelBuilder.Entity<JAccountTask>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.TaskId });

                entity.ToTable("J_AccountTask");

                entity.Property(e => e.AccountId).HasColumnName("Account_ID");

                entity.Property(e => e.TaskId).HasColumnName("Task_ID");

                entity.Property(e => e.TaskCreator).HasColumnName("Task_Creator");

                entity.Property(e => e.TaskNotification).HasColumnName("Task_Notification");

                entity.Property(e => e.UserAssignedDateTime)
                    .HasColumnName("User_Assigned_DateTime")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.JAccountTask)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__J_Account__Accou__38996AB5");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.JAccountTask)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__J_Account__Task___398D8EEE");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).HasColumnName("Project_ID");

                entity.Property(e => e.ProjectActive).HasColumnName("Project_Active");

                entity.Property(e => e.ProjectCreatedDateTime)
                    .HasColumnName("Project_Created_DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProjectCreatorAccountId).HasColumnName("Project_Creator_Account_ID");

                entity.Property(e => e.ProjectDeadline)
                    .HasColumnName("Project_Deadline")
                    .HasColumnType("date");

                entity.Property(e => e.ProjectDescription)
                    .HasColumnName("Project_Description")
                    .HasMaxLength(4000);

                entity.Property(e => e.ProjectEndDateTime)
                    .HasColumnName("Project_End_DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnName("Project_Name")
                    .HasMaxLength(150);

                entity.HasOne(d => d.ProjectCreatorAccount)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.ProjectCreatorAccountId)
                    .HasConstraintName("FK__Project__Project__3A81B327");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.TaskId).HasColumnName("Task_ID");

                entity.Property(e => e.TaskCreatedDateTime)
                    .HasColumnName("Task_Created_DateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.TaskDescription)
                    .HasColumnName("Task_Description")
                    .HasMaxLength(3000);

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasColumnName("Task_Name")
                    .HasMaxLength(200);

                entity.Property(e => e.TaskProjectId).HasColumnName("Task_Project_ID");

                entity.Property(e => e.TaskTaskStateId).HasColumnName("Task_TaskState_ID");

                entity.Property(e => e.TaskTaskStateLastChangeDateTime)
                    .HasColumnName("Task_TaskState_LastChange_DateTime")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.TaskTaskState)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.TaskTaskStateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Task__Task_TaskS__3B75D760");
            });

            modelBuilder.Entity<TaskState>(entity =>
            {
                entity.Property(e => e.TaskStateId).HasColumnName("TaskState_ID");

                entity.Property(e => e.TaskStateName)
                    .IsRequired()
                    .HasColumnName("TaskState_Name")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.UserRoleId).HasColumnName("UserRole_ID");

                entity.Property(e => e.UserRoleName)
                    .HasColumnName("UserRole_Name")
                    .HasMaxLength(150);
            });
        }
    }
}
