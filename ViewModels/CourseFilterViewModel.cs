using FacultyMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.ViewModels
{
    public class CourseFilterViewModel
    {
        public IList<Course> Courses { get; set; }
        public SelectList Semesters { get; set; } //select lista od site postoecki semestri za izbiranje na eden semestar
        public int CourseSemester { get; set; } //za filtriranjeto po Semester
        public SelectList Programmes { get; set; } //select lista od site postoecki programi za izbiranje na edna programa
        public string CourseProgramme { get; set; } //za filtriranjeto po Programme
        public string SearchString { get; set; }  //za filtriranjeto po Title
        public string TeacherString { get; set; } //za filtriranjeto po Teacher
    }
}
