using FacultyMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.ViewModels
{
    public class TeacherFilterViewModel
    {
        public IList<Teacher> Teachers { get; set; }
        public SelectList Ranges { get; set; } 
        public string TeacherAcademicRank { get; set; } 
        public SelectList Degrees { get; set; }
        public string TeacherDegree{ get; set; } 
        public string SearchString { get; set; }  //za filtriranjeto po FullName
    }
}
