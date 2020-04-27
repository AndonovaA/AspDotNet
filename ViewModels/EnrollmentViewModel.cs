﻿using FacultyMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.ViewModels
{
    public class EnrollmentViewModel
    {
        public Enrollment NewEnrollment { get; set; }
        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<SelectListItem> StudentsList { get; set; }
    }
}
