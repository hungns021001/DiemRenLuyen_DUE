using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiemNgoaiKhoa.Helpers;
using DiemNgoaiKhoa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DiemNgoaiKhoa.Controllers
{
    [Authorize]

    public class StudentsController : Controller
    {
        private readonly DataContext _context;

        public StudentsController(DataContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;
            var giangvien = _context.Classes.Where(a => a.Lecturer.Account.Username == username).FirstOrDefault();

            if (role == "admin")
            {
                return View(await _context.Students.Include(l => l.Account).Include(l => l.Gender).ToListAsync());
            }
            else if(role =="giảng viên")
            {
                return View(await _context.Students.Include(l => l.Account).Include(l => l.Gender).Where(a => a.ClassId == giangvien.Id).ToListAsync());
            }    
            else
            {
                return View(await _context.Students.Include(l => l.Account).Include(l => l.Gender).Where(a => a.Account.Username == username).ToListAsync());
            }
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Account)
                .Include(s => s.Class)
                .Include(s => s.Gender)
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name");
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> Create(StudentRequest request)
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;

            Student student = new Student();

            var exist = await _context.Students.AnyAsync(a => a.AccountId == request.AccountId);
            if (ModelState.IsValid && !exist)
            {
                student.Fullname = request.Fullname;
                student.Birthday = request.Birthday;
                student.GenderId = request.GenderId;
                student.Phone = request.Phone;
                student.Address = request.Address;
                student.ClassId = request.ClassId;
                student.AccountId = request.AccountId;
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a=>a.Roles.Name == "sinh viên"), "Id", "Username", student.AccountId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", student.ClassId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", student.GenderId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", student.AccountId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", student.ClassId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", student.GenderId);
            return View(student);
        }

        public async Task<Student> GetById(int id)
        {
            return await this._context.Students.Where(c => c.Id == id).FirstAsync();
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentRequest request)
        {
            Student student = await this.GetById(id);
            var exist = await _context.Students.AnyAsync(a => a.AccountId == request.AccountId);

            if (id != student.Id && !exist)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    student.Fullname = request.Fullname;
                    student.Birthday = request.Birthday;
                    student.GenderId = request.GenderId;
                    student.Phone = request.Phone;
                    student.Address = request.Address;
                   // student.ClassId = request.ClassId;
                   // student.AccountId = request.AccountId;
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", student.AccountId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Name", student.ClassId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", student.GenderId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Account)
                .Include(s => s.Class)
                .Include(s => s.Gender)
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
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'DataContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return (_context.Students?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
