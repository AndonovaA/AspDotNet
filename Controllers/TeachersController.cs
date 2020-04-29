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
    public class TeachersController : Controller
    {
        private readonly FacultyMVCContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeachersController(FacultyMVCContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Teachers
        public async Task<IActionResult> Index(string teacherAcademicRank, string teacherDegree, string searchString)
        {

            //IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();  
            IEnumerable<Teacher> teachers = _context.Teacher;
            IQueryable<string> rankQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();
            IQueryable<string> degreeQuery = _context.Teacher.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();

            if (!string.IsNullOrEmpty(teacherAcademicRank))
            {
                teachers = teachers.Where(x => x.AcademicRank == teacherAcademicRank);
            }
            if (!string.IsNullOrEmpty(teacherDegree))
            {
                teachers = teachers.Where(x => x.Degree == teacherDegree);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                teachers = teachers.ToList().Where(s => s.FullName.ToLower().Contains(searchString.ToLower()));
                //teachers = teachers.Where(s => s.FullName.Contains(searchString));  //ne raboti
            }


            var teacherViewModel = new TeacherFilterViewModel
            {
                Ranges = new SelectList(await rankQuery.ToListAsync()),
                Degrees = new SelectList(await degreeQuery.ToListAsync()),
                Teachers = teachers.ToList()
            };

            return View(teacherViewModel);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Teacher teacher = new Teacher
                {
                    
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Degree = model.Degree,
                    AcademicRank = model.AcademicRank,
                    OfficeNumber = model.OfficeNumber,
                    HireDate = model.HireDate,
                    ProfilePicture = uniqueFileName,
                    Courses_first = model.Courses_first,
                    Courses_second = model.Courses_second
                };

                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        private string UploadedFile(TeacherFormViewModel model)
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

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            TeacherFormViewModel vm = new TeacherFormViewModel
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Degree = teacher.Degree,
                AcademicRank = teacher.AcademicRank,
                OfficeNumber = teacher.OfficeNumber,
                HireDate = teacher.HireDate,
                Courses_first = teacher.Courses_first,
                Courses_second = teacher.Courses_second
            };

            return View(vm);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherFormViewModel vm)
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

                    Teacher teacher = new Teacher
                    {
                        Id = vm.Id,
                        FirstName = vm.FirstName,
                        LastName = vm.LastName,
                        ProfilePicture = uniqueFileName,
                        Degree = vm.Degree,
                        AcademicRank = vm.AcademicRank,
                        OfficeNumber = vm.OfficeNumber,
                        HireDate = vm.HireDate,
                        Courses_first = vm.Courses_first,
                        Courses_second = vm.Courses_second
                    };

                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(vm.Id))
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

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }

        // GET: Teachers/GetCourses/2
        public async Task<IActionResult> GetCourses(int id)
        {
            var courses = _context.Course.Where(c=>c.FirstTeacherId == id || c.SecondTeacherId == id);
            courses = courses.Include(t=>t.FirstTeacher).Include(t=>t.SecondTeacher);

            ViewData["TeacherId"] = id;
            return View(courses);
        }
    }
}
