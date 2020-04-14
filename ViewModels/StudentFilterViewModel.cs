using FacultyMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;


namespace FacultyMVC.ViewModels
{
    public class StudentFilterViewModel
    {
        public IList<Student> Students { get; set; }
        public SelectList Indexes { get; set; } 
        public string StudentIndex { get; set; }  
        public string SearchString { get; set; }
        public string CourseString { get; set; }  //pregled na studenti po predmet
    }
}
