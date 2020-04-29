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

namespace FacultyMVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly FacultyMVCContext _context;

        public CoursesController(FacultyMVCContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(int courseSemester, string courseProgramme, string searchString/*, string teacherString*/)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable(); //lista od queries za sekoj course
            IQueryable<int> semesterQuery= _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct(); //lista od queries za site mozni semestri
            IQueryable<string> programmeQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct(); //lista od queries za site mozni programi

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Title.ToLower().Contains(searchString.ToLower()));
            }

            //eager loading
            courses = courses.Include(m => m.FirstTeacher)
                .Include(m => m.SecondTeacher);

           /* IEnumerable<Course> dataList = courses as IEnumerable<Course>;

            if (!string.IsNullOrEmpty(teacherString))
            {
                dataList = dataList.Where(s => s.FirstTeacher.FullName.ToLower().Contains(teacherString.ToLower()) || s.SecondTeacher.FullName.ToLower().Contains(teacherString.ToLower())); 
            }*/

            if (courseSemester != 0) 
            {
                /*dataList = dataList.Where(x => x.Semester == courseSemester);*/
                courses = courses.Where(x => x.Semester == courseSemester);
            }
            if (!string.IsNullOrEmpty(courseProgramme))
            {
                /*dataList = dataList.Where(x => x.Programme == courseProgramme);*/
                courses = courses.Where(x => x.Programme == courseProgramme);
            }


            var courseViewModel = new CourseFilterViewModel
            {
                Semesters = new SelectList(await semesterQuery.ToListAsync()), //dobivanje lista od semesters
                Programmes = new SelectList(await programmeQuery.ToListAsync()),
                /*Courses = dataList.ToList() //dobivanje lista*/
                Courses = await courses.ToListAsync()
            };
       
            return View(courseViewModel);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                 .Include(c => c.FirstTeacher)
                 .Include(c => c.SecondTeacher)
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);

            //Faza1
            /* var course = _context.Course.Where(m => m.Id == id).Include(m => m.Students).First();*/
            
            if (course == null)
            {
                return NotFound();
            }

            //Faza1
           /* CourseStudentViewModel vm = new CourseStudentViewModel
            {
                Course = course,
                StudentsList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId) 
            };*/

            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);

            //Faza1
            /*return View(vm);*/
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*CourseStudentViewModel viewmodel*/ [Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (id != course.Id /*viewmodel.Course.Id*/)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /* _context.Update(viewmodel.Course); */
                    _context.Update(course);
                    await _context.SaveChangesAsync();

                    //Faza1
                    /*IEnumerable<int> listStudents = viewmodel.SelectedStudents;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<int> existStudents = _context.Enrollment.Where(s=> listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s=>s.StudentId);
                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                    foreach (int studentId in newStudents)
                        _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id });*/

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists( course.Id /*viewmodel.Course.Id*/))
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
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", /*viewmodel.Course.*/course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(/*viewmodel*/ course );
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Upsert/3
        public async Task<IActionResult> Upsert(int? id)
        {
            var course = _context.Course.Where(m => m.Id == id).Include(m => m.Students).First();

            EnrollmentViewModel vm = new EnrollmentViewModel
            {
                StudentsList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId)
            };

            ViewData["chosenId"] = id;
            return View(vm);
        }

        // POST: Courses/Upsert/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int id, EnrollmentViewModel viewmodel)
        { 
            if (id != viewmodel.NewEnrollment.CourseId)
            {
                return NotFound();
            }

            //Insert (enroll students)
            if (viewmodel.NewEnrollment.FinishDate == null)
            {
                IEnumerable<int> listStudents = viewmodel.SelectedStudents;
                IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));

                foreach (int studentId in newStudents)
                    _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, Year = viewmodel.NewEnrollment.Year, Semester = viewmodel.NewEnrollment.Semester });

                await _context.SaveChangesAsync();
            }
            else
            {
                //Update enrollments (write off students) 
                var enrollments = _context.Enrollment.Where(e => e.CourseId == id).Include(e => e.Course).Include(e => e.Student);

                foreach (Enrollment e in enrollments) {
                    e.FinishDate = viewmodel.NewEnrollment.FinishDate;
                }

                _context.Enrollment.UpdateRange(enrollments);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }


        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
