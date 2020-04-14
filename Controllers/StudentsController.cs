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
    public class StudentsController : Controller
    {
        private readonly FacultyMVCContext _context;

        public StudentsController(FacultyMVCContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(string studentIndex, string searchString, string courseString)
        {
            IQueryable<Student> students = _context.Student.AsQueryable();
            IQueryable<string> indexQuery = _context.Student.OrderBy(m => m.Index).Select(m => m.Index).Distinct();

            students = students.Include(s => s.Courses).ThenInclude(s => s.Course);

            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();
            enrollments = enrollments.Include(e => e.Course).Include(e => e.Student);

            if (!string.IsNullOrEmpty(courseString))
            {
                enrollments = enrollments.Where(s => s.Course.Title.Contains(courseString)); //se zemaat onie zapisi kaj koi naslovot na kursot e courseString
                IEnumerable<int> enrollmentsIDS = enrollments.Select(e => e.StudentId).Distinct(); //se zemaat distinct IDs na studentite od prethodno najdenite zapisi
                students = students.Where(s => enrollmentsIDS.Contains(s.Id));  //filtriranje na students spored id
            }

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
        public async Task<IActionResult> Create([Bind("Id, Index, FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
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
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Index, FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
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
    }
}
