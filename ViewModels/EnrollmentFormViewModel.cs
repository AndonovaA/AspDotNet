﻿using FacultyMVC.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FacultyMVC.ViewModels
{
    public class EnrollmentFormViewModel
    {
        public int Id { get; set; }

        [StringLength(10)]
        public string Semester { get; set; }

        public int? Year { get; set; }

        public int? Grade { get; set; }

        public IFormFile SeminalUrl { get; set; }

        [StringLength(255)]
        public string ProjectUrl { get; set; }

        public int? ExamPoints { get; set; }
        public int? SeminalPoints { get; set; }
        public int? ProjectPoints { get; set; }
        public int? AdditionalPoints { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }

        public int CourseId { get; set; }
        public int StudentId { get; set; }
    }
}
