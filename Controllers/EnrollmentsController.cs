using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FacultyMVC.Data;
using FacultyMVC.Models;
using FacultyMVC.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FacultyMVC.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly FacultyMVCContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EnrollmentsController(FacultyMVCContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index()
        {
            /* var facultyMVCContext = _context.Enrollment.Include(e => e.Course).Include(e => e.Student);
             return View(await facultyMVCContext.ToListAsync());*/

            IQueryable<Course> facultyMVCContext = _context.Course.OrderBy(m => m.Semester).AsQueryable();
            
            return View(await facultyMVCContext.ToListAsync());
        }

        // GET: Enrollments/CourseStudents/5
        public async Task<IActionResult> CourseStudents(int? id)
        { 
            if (id == null)
            {
                return NotFound();
            }

            IQueryable<Enrollment> enrollment = _context.Enrollment.Where(e => e.CourseId == id);
            enrollment = enrollment.Include(e => e.Course).Include(e => e.Student).OrderBy(e=>e.Student.Index);
    
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(await enrollment.ToListAsync());
        }

        // GET: Enrollments/Create/3
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var enrollments = _context.Enrollment.Where(e=>e.CourseId==id).Include(e => e.Course).Include(e => e.Student);
            var course = _context.Course.Where(m => m.Id == id).Include(m => m.Students).First();

            EnrollmentViewModel vm = new EnrollmentViewModel
            {
                StudentsList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId)
            };

            ViewData["chosenId"] = id;
            /*ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName");*/
            return View(vm);
        }

        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, EnrollmentViewModel viewmodel)
        {
            if (id != viewmodel.NewEnrollment.CourseId)
            {
                return NotFound();
            }

            IEnumerable<int> listStudents = viewmodel.SelectedStudents;
            IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
            _context.Enrollment.RemoveRange(toBeRemoved);

            IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
            IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
            foreach (int studentId in newStudents)
                _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, Year = viewmodel.NewEnrollment.Year, Semester = viewmodel.NewEnrollment.Semester });

            await _context.SaveChangesAsync();

            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", viewmodel.NewEnrollment.CourseId);
            return RedirectPermanent("~/Courses/Index");
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,CourseId,StudentId")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("CourseStudents", new { id = enrollment.CourseId });
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = enrollment.CourseId });
        }

        // GET: Enrollments/EditByStudent/5
        public async Task<IActionResult> EditByStudent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            } 

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            EnrollmentFormViewModel vm = new EnrollmentFormViewModel
            {
                Id = enrollment.Id,
                Semester = enrollment.Semester,
                Year = enrollment.Year,
                Grade = enrollment.Grade,
                ProjectUrl = enrollment.ProjectUrl,
                SeminalPoints = enrollment.SeminalPoints,
                ProjectPoints = enrollment.ProjectPoints,
                AdditionalPoints = enrollment.AdditionalPoints,
                ExamPoints = enrollment.ExamPoints,
                FinishDate = enrollment.FinishDate,
                CourseId = enrollment.CourseId,
                StudentId = enrollment.StudentId
            };

            return View(vm);
        }

        // POST: Enrollments/EditByStudent/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditByStudent(int id, EnrollmentFormViewModel vm)
        { 
            if (id != vm.Id) 
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadedFile(vm);

                    Enrollment enrollment= new Enrollment
                    {
                        Id=vm.Id,
                        Semester= vm.Semester,
                        Year= vm.Year,
                        Grade= vm.Grade,
                        SeminalUrl = uniqueFileName,
                        ProjectUrl = vm.ProjectUrl,
                        SeminalPoints=vm.SeminalPoints,
                        ProjectPoints=vm.ProjectPoints,
                        AdditionalPoints=vm.AdditionalPoints,
                        ExamPoints=vm.ExamPoints,
                        FinishDate=vm.FinishDate,
                        CourseId=vm.CourseId,
                        StudentId=vm.StudentId
                    };

                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(vm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = vm.Id });
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", vm.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", vm.StudentId);
            return View(vm);
        }

        private string UploadedFile(EnrollmentFormViewModel model)
        {
            string uniqueFileName = null;

            if (model.SeminalUrl != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "seminalFiles");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.SeminalUrl.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SeminalUrl.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
