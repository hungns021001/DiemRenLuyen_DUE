using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiemNgoaiKhoa.Helpers;
using DiemNgoaiKhoa.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace DiemNgoaiKhoa.Controllers
{
    [Authorize]
    public class PointsController : Controller
    {
        private readonly DataContext _context;

        public PointsController(DataContext context)
        {
            _context = context;
        }

        // GET: Points
        public async Task<IActionResult> Index(int? student, int? semester, int? classes)
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;

            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name");
            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name");
            ViewData["ItemDetailId"] = new SelectList(_context.ItemDetails, "Id", "Name");
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name");
            var lopt =  _context.Students.Where(a=>a.Account.Username== username).FirstOrDefault();
            var giangvien =  _context.Classes.Where(a=>a.Lecturer.Account.Username == username).FirstOrDefault();

            if (role == "sinh viên")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.Account.Username == username), "Id", "Fullname");
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(a=>a.Id == lopt.ClassId), "Id", "Name");
            }
            else if (role == "giảng viên")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.ClassId == classes), "Id", "Fullname");
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(a => a.Lecturer.Account.Username == username), "Id", "Name");
            }
            else if (role == "lớp trưởng")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.ClassId == classes), "Id", "Fullname");
                ViewData["ClassId"] = new SelectList(_context.Classes.Where(a=>a.Id == lopt.ClassId), "Id", "Name");
            }
            else if (role =="admin")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.ClassId == classes), "Id", "Fullname");
                ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name");
            }

            //IQueryable<Point> dataContext = _context.Points.Include(p => p.PointFrame).Include(p => p.Semester).Include(p => p.Student);
            //if (student != null && semester != null)
            //{
            //    dataContext = _context.Points.Where(a=>a.StudentId== student && a.SemesterId == semester);
            //}    
            //return View(await dataContext.ToListAsync());
            IQueryable<Point> dataContext = _context.Points.Select(o => new Point()
            {
                Id = o.Id,
                Student = new Student()
                {
                    Fullname = o.Student.Fullname,
                    Class = new Class()
                    {
                        Name = o.Student.Class.Name,
                    },
                },
                Semester = new Semester()
                {
                    Name = o.Semester.Name
                },
                PointFrame = new PointFrame()
                {
                    ItemDetail = new ItemDetail()
                    {
                        Item = new Item()
                        {
                            Name = o.PointFrame.ItemDetail.Item.Name,
                        },
                        Name = o.PointFrame.ItemDetail.Name
                    },
                    Name = o.PointFrame.Name,
                    MaxPoint = o.PointFrame.MaxPoint
                },
                PointStudent = o.PointStudent
            });
            if (student != null && semester != null)
            {
                dataContext = _context.Points.Where(a => a.StudentId == student && a.SemesterId == semester && a.Student.Class.Id == classes);
            }
            if (role == "sinh viên")
            {
                dataContext = _context.Points.Where(a => a.Student.Account.Username == username && a.SemesterId == semester && a.Student.Class.Id == classes);
            }
            else if (role =="lớp trưởng")
            {
                dataContext = _context.Points.Where(a => a.StudentId == student && a.SemesterId == semester && a.Student.Class.Id == lopt.ClassId);
            }
            else if (role == "giảng viên")
            {
                dataContext = _context.Points.Where(a => a.StudentId == student && a.SemesterId == semester && a.Student.Class.Id == giangvien.Id);
            }
            else
            {
                dataContext = _context.Points.Where(a => a.StudentId == student && a.SemesterId == semester && a.Student.Class.Id == classes);
            }
            return View(await dataContext.OrderBy(o=> o.PointFrame.ItemDetail.ItemId).ThenBy(o=>o.PointFrame.ItemDetailId).ToListAsync());

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
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;

            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name");
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name");
            if (role == "sinh viên")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.Account.Username == username), "Id", "Fullname");
            }
            else
            {
                ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname");
            }
            return View();
        }

        // POST: Points/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PointRequest request)
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;
            var max = _context.PointFrames.Where(a=> a.Id == request.PointFrameId).FirstOrDefault();
            //var exist = _context.Points.Any(a=>a.PointFrameId== request.PointFrameId && a.SemesterId == request.SemesterId); //đang lỗi nè

            Point point = new Point();
            if (ModelState.IsValid)
            {
                if (0 <= request.PointStudent && request.PointStudent <= max.MaxPoint)
                {
                    point.StudentId = request.StudentId;
                    point.PointFrameId = request.PointFrameId;
                    point.SemesterId = request.SemesterId;
                    point.PointStudent = request.PointStudent;
                    point.PointMonitor = 0;
                    point.PointLecturer = 0;
                    if (role == "admin" && 0 <= request.PointMonitor && request.PointMonitor <= max.MaxPoint && 0 <= request.PointLecturer && request.PointLecturer <= max.MaxPoint)
                    {
                        point.PointMonitor = request.PointMonitor;
                        point.PointLecturer = request.PointLecturer;
                    }
                    _context.Add(point);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["PointFrameId"] = new SelectList(_context.PointFrames, "Id", "Name", point.PointFrameId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", point.SemesterId);
            ViewData["StudentId"] = new SelectList(_context.Students.Where(a=> a.Account.Username == username), "Id", "Fullname", point.StudentId);
            return View(point);
        }

        // GET: Points/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;
            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;
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
            if (role == "sinh viên")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.Account.Username == username), "Id", "Fullname");

            }
            else
            {
                ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname");
            }
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
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;
            var max = _context.PointFrames.Where(a => a.Id == request.PointFrameId).FirstOrDefault();

            Point point = await this.GetById(id);
            if (id != point.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //point.StudentId = request.StudentId;
                    if (role == "sinh viên" /*|| role == "lớp trưởng" || role == "admin"*/)
                    {
                        if (0 <= request.PointStudent && request.PointStudent <= max.MaxPoint)
                        {
                            point.PointStudent = request.PointStudent;
                        }
                    }
                    //point.SemesterId = request.SemesterId;
                    else if (role == "lớp trưởng" /*|| role == "admin"*/)
                    {
                        if (0 <= request.PointMonitor && request.PointMonitor <= max.MaxPoint)
                        {
                            point.PointMonitor = request.PointMonitor;
                        }
                    }
                    else if (role == "giảng viên" || role == "admin")
                    {
                        if (0 <= request.PointMonitor && request.PointMonitor <= max.MaxPoint)
                        {
                            point.PointLecturer = request.PointLecturer;
                        }
                    }
                    //point.PointFrameId = request.PointFrameId;
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
            if (role == "sinh viên")
            {
                ViewData["StudentId"] = new SelectList(_context.Students.Where(a => a.Account.Username == username), "Id", "Fullname", point.StudentId);
            }
            else
            {
                ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Fullname", point.StudentId);
            }
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
        [Authorize(Roles = "admin")]
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
