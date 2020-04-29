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
    public class StudentsController : Controller
    {
        private readonly FacultyMVCContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public StudentsController(FacultyMVCContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Students
        public async Task<IActionResult> Index(string studentIndex, string searchString)
        {
            IQueryable<Student> students = _context.Student.AsQueryable();
            IQueryable<string> indexQuery = _context.Student.OrderBy(m => m.Index).Select(m => m.Index).Distinct();

            students = students.Include(s => s.Courses).ThenInclude(s => s.Course);

            if (!string.IsNullOrEmpty(studentIndex))
            {
                students = students.Where(x => x.Index == studentIndex);
            }

            IEnumerable<Student> dataList = students as IEnumerable<Student>;

            if (!string.IsNullOrEmpty(searchString)) 
            {
                dataList = dataList.ToList().Where(s => s.FullName.ToLower().Contains(searchString.ToLower()));
            }

            var studentViewModel = new StudentFilterViewModel
            {
                Indexes = new SelectList(await indexQuery.ToListAsync()),
                Students = dataList.ToList()
            };

            return View(studentViewModel);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Student student = new Student
                {
                    Index = model.Index,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EnrollmentDate = model.EnrollmentDate,
                    AcquiredCredits = model.AcquiredCredits,
                    CurrentSemestar = model.CurrentSemestar,
                    ProfilePicture = uniqueFileName,
                    EducationLevel = model.EducationLevel,
                    Courses= model.Courses
                };

                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string UploadedFile(StudentFormViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfilePicture.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfilePicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            StudentFormViewModel vm = new StudentFormViewModel
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Index = student.Index,
                EnrollmentDate = student.EnrollmentDate,
                AcquiredCredits = student.AcquiredCredits,
                CurrentSemestar = student.CurrentSemestar,
                EducationLevel = student.EducationLevel,
                Courses = student.Courses
            };

            return View(vm);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentFormViewModel vm)
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

                    Student student = new Student
                    {
                        Id = vm.Id,
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        ProfilePicture = uniqueFileName,
                        EnrollmentDate = vm.EnrollmentDate,
                        CurrentSemestar = vm.CurrentSemestar,
                        AcquiredCredits = vm.AcquiredCredits,
                        Index = vm.Index,
                        EducationLevel = vm.EducationLevel,
                        Courses = vm.Courses
                    };

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(vm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }

        // GET: Students/MyCourses/2
        public async Task<IActionResult> MyCourses(int? id)
        {
            IQueryable<Course> courses = _context.Course.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).AsQueryable();

            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();
            enrollments = enrollments.Where(s => s.StudentId==id); //se zemaat onie zapisi kaj koi studentId == id-to od url-to
            IEnumerable<int> enrollmentsIDS = enrollments.Select(e => e.CourseId).Distinct(); //se zemaat distinct IDs na courses od prethodno najdenite zapisi

            courses = courses.Where(s => enrollmentsIDS.Contains(s.Id));  //filtriranje na students spored id

            courses = courses.Include(c => c.Students).ThenInclude(c => c.Student);

            ViewData["studentId"] = id;
            return View(courses);
        }
    }
}
