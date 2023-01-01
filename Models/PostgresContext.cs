using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClassTeacher_Assist.Models;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
        
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Skip> Skips { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }
    
    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Teaching> Teachings { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=course;Username=postgres;Password=evalone19200319;Include Error Detail=true");
        optionsBuilder.EnableSensitiveDataLogging(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_catalog", "adminpack");

        modelBuilder.Entity<Teaching>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.TeacherId }).HasName("teaching_pkey");

            entity.ToTable("teaching");

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Class).WithMany(p => p.Teachings)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("fk_class_id");
            entity.HasOne(d => d.Teacher).WithMany(p => p.Teachings)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("fk_teacher_id"); ;
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("pk_class_id");

            entity.ToTable("class");

            entity.HasIndex(e => e.ClassCode, "unique_class_code").IsUnique();

            entity.HasIndex(e => e.TeacherId, "unique_teacher_id").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassCode).HasColumnName("class_code");
            entity.Property(e => e.StudentsNumber).HasColumnName("students_number");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Teacher).WithOne(p => p.Class)
                .HasForeignKey<Class>(d => d.TeacherId)
                .HasConstraintName("fk_teacher_id");

        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("pk_grade_id");

            entity.ToTable("grade");

            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.ReceivingDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("receiving_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_student_id");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Grades)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("fk_teacher_id");
        });

        modelBuilder.Entity<Skip>(entity =>
        {
            entity.HasKey(e => e.SkipId).HasName("pk_skip_id");

            entity.ToTable("skip");

            entity.Property(e => e.SkipId).HasColumnName("skip_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.ReceivingDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("receiving_date");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.HasOne(d => d.Student).WithMany(p => p.Skips)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("fk_student_id");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Skips)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("fk_teacher_id");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("pk_student_id");

            entity.ToTable("student");

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Patronymic).HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("fk_class_id");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("pk_subject_id");

            entity.ToTable("subject");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("pk_teacher_id");

            entity.ToTable("teacher");

            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Patronymic).HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("fk_subject_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
