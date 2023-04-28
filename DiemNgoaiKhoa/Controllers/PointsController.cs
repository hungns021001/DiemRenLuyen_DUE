using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiemNgoaiKhoa.Helpers;
using DiemNgoaiKhoa.Models;

namespace DiemNgoaiKhoa.Controllers
{
    public class PointsController : Controller
    {
        private readonly DataContext _context;

        public PointsController(DataContext context)
        {
            _context = context;
        }

        // GET: Points
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Points.Include(p => p.PointFrame).Include(p => p.Semester).Include(p => p.Student);
            return View(await dataContext.ToListAsync());
            //var dataContext = _context.Points.Where(o => o.Student.Id == o.Id).Select(o =>  new Point()
            //{
            //    Id = o.Id,
            //    Student = new Student()
            //    {
            //        Fullname = o.Student.Fullname
            //    },
            //    Semester = new Semester()
            //    {
            //        Name = o.Semester.Name
            //    },
            //    PointFrame = new PointFrame()
            //    {
            //        Name = o.PointFrame.Name
            //    },
            //    PointStudent = o.PointStudent + o.PointStudent
            //});
            //return View(dataContext.ToListAsync());
            
        }

        // GET: Points/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Points == null)
            {
                return NotFound();
            }

            var point = await _context.Points
                .Include(p => p.PointFrame)
                .Include(p => p.Semester)
                .Include(p => p.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (point == null)
            {
                return NotFound();
            }

            return View(point);
        }

        // GET: Points/Create
        public IActionResult Create()
        {
            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name");
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname");
            return View();
        }

        // POST: Points/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PointRequest request)
        {
            Point point = new Point();
            if (ModelState.IsValid)
            {
                point.StudentId = request.StudentId;
                point.PointStudent = request.PointStudent;
                point.SemesterId = request.SemesterId;
                point.PointMonitor = request.PointMonitor;
                point.PointLecturer = request.PointLecturer;
                point.PointFrameId = request.PointFrameId;
                _context.Add(point);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name", point.PointFrameId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", point.SemesterId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname", point.StudentId);
            return View(point);
        }

        // GET: Points/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Points == null)
            {
                return NotFound();
            }

            var point = await _context.Points.FindAsync(id);
            if (point == null)
            {
                return NotFound();
            }
            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name", point.PointFrameId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", point.SemesterId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname", point.StudentId);
            return View(point);
        }

        public async Task<Point> GetById(int id)
        {
            return await this._context.Points.Where(c => c.Id == id).FirstAsync();
        }

        // POST: Points/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PointRequest request)
        {
            Point point = await this.GetById(id);
            if (id != point.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    point.StudentId = request.StudentId;
                    point.PointStudent = request.PointStudent;
                    point.SemesterId = request.SemesterId;
                    point.PointMonitor = request.PointMonitor;
                    point.PointLecturer = request.PointLecturer;
                    point.PointFrameId = request.PointFrameId;
                    _context.Update(point);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointExists(point.Id))
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
            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name", point.PointFrameId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", point.SemesterId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname", point.StudentId);
            return View(point);
        }

        // GET: Points/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Points == null)
            {
                return NotFound();
            }

            var point = await _context.Points
                .Include(p => p.PointFrame)
                .Include(p => p.Semester)
                .Include(p => p.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (point == null)
            {
                return NotFound();
            }

            return View(point);
        }

        // POST: Points/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Points == null)
            {
                return Problem("Entity set 'DataContext.Points'  is null.");
            }
            var point = await _context.Points.FindAsync(id);
            if (point != null)
            {
                _context.Points.Remove(point);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PointExists(int id)
        {
          return (_context.Points?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
