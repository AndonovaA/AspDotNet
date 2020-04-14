using FacultyMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.ViewModels
{
    public class CourseTeacherViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<SelectListItem> TeachersList { get; set; }
    }
}
