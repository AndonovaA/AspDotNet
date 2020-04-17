using FacultyMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.Models
{
    public class SeedData
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FacultyMVCContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<FacultyMVCContext>>()))
            {
                if (context.Course.Any() || context.Enrollment.Any() || context.Student.Any() || context.Teacher.Any())
                {
                    return;
                }

                context.Course.AddRange(
                new Course { Title = "PIA", Credits = 6, Semester = 1, FirstTeacherId = 1  , SecondTeacherId = 2}
                );
                context.SaveChanges();

                context.Teacher.AddRange(
                new Teacher { FirstName = "Billy", LastName = "Crystal"}
                );
                context.SaveChanges();

                context.Student.AddRange(new Student
                {
                    Index = "10/2017",
                    FirstName = "Tina",
                    LastName = "Petkova"
                }
            );
                context.SaveChanges(); context.Enrollment.AddRange(
                    new Enrollment { CourseId = 3, StudentId = 2},
                    new Enrollment { CourseId = 3, StudentId = 7}
                );
                context.SaveChanges();
            }
        }

    }
}
