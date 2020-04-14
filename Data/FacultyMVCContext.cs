using FacultyMVC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.Data
{
    public class FacultyMVCContext : DbContext
    {
        public FacultyMVCContext(DbContextOptions <FacultyMVCContext> options):base(options)
        { }

        public DbSet<Course> Course { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
         
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Teacher>()
                .HasMany<Course>(p => p.Courses_first)
                .WithOne(p => p.FirstTeacher)
                .HasForeignKey(p => p.FirstTeacherId);

            builder.Entity<Teacher>()
                .HasMany<Course>(p => p.Courses_second)
                .WithOne(p => p.SecondTeacher)
                .HasForeignKey(p => p.SecondTeacherId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Enrollment>()
                .HasOne<Student>(p => p.Student)
                .WithMany(p => p.Courses)
                .HasForeignKey(p => p.StudentId);

            builder.Entity<Enrollment>()
               .HasOne<Course>(p => p.Course)
               .WithMany(p => p.Students)
               .HasForeignKey(p => p.CourseId);
        }

    }
}
