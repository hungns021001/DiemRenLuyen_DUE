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
    [Authorize(Roles = "admin,giảng viên")]
    public class LecturersController : Controller
    {
        private readonly DataContext _context;

        public LecturersController(DataContext context)
        {
            _context = context;
        }

        // GET: Lecturers
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity.FindFirst(ClaimTypes.Role)?.Value;
            
            if(role == "admin")
            {
                return View(await _context.Lecturer.Include(l => l.Account).Include(l => l.Gender).ToListAsync());
            }
            else
            {
                return View(await _context.Lecturer.Include(l => l.Account).Include(l => l.Gender).Where(a=>a.Account.Username == username).ToListAsync());
            }

        }

        // GET: Lecturers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lecturer == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturer
                .Include(l => l.Account)
                .Include(l => l.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecturer == null)
            {
                return NotFound();
            }

            return View(lecturer);
        }

        // GET: Lecturers/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username");
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name");
            return View();
        }

        // POST: Lecturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(LecturerRequest request)
        {
            var user = HttpContext.User;
            var identity = user.Identity as ClaimsIdentity;

            var username = identity.FindFirst(ClaimTypes.Name)?.Value;

            Lecturer lecturer = new Lecturer();
            var exist = await _context.Lecturer.AnyAsync(a=>a.AccountId == request.AccountId);
            if (ModelState.IsValid && !exist)
            {
                lecturer.Fullname = request.Fullname;
                lecturer.Birthday = request.Birthday;
                lecturer.Address = request.Address;
                lecturer.Phone = request.Phone;
                lecturer.GenderId = request.GenderId;
                lecturer.AccountId = request.AccountId;
                _context.Add(lecturer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a=> a.RoleId == 2 ), "Id", "Username", lecturer.AccountId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", lecturer.GenderId);
            return View(lecturer);
        }

        // GET: Lecturers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Lecturer == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturer.FindAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", lecturer.AccountId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", lecturer.GenderId);
            return View(lecturer);
        }

        public async Task<Lecturer> GetById (int id)
        {
            return await this._context.Lecturer.Where(c => c.Id == id).FirstAsync();
        }

        // POST: Lecturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LecturerRequest request)
        {
            Lecturer lecturer = await this.GetById(id);
            var exist = await _context.Lecturer.AnyAsync(a => a.AccountId == request.AccountId);

            if (id != lecturer.Id || exist)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    lecturer.Fullname = request.Fullname;
                    lecturer.Birthday = request.Birthday;
                    lecturer.Address = request.Address;
                    lecturer.Phone = request.Phone;
                    lecturer.GenderId = request.GenderId;
                    //lecturer.AccountId = request.AccountId;
                    _context.Update(lecturer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LecturerExists(lecturer.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Username", lecturer.AccountId);
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", lecturer.GenderId);
            return View(lecturer);
        }

        // GET: Lecturers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lecturer == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturer
                .Include(l => l.Account)
                .Include(l => l.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecturer == null)
            {
                return NotFound();
            }

            return View(lecturer);
        }

        // POST: Lecturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lecturer == null)
            {
                return Problem("Entity set 'DataContext.Lecturer'  is null.");
            }
            var lecturer = await _context.Lecturer.FindAsync(id);
            if (lecturer != null)
            {
                _context.Lecturer.Remove(lecturer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LecturerExists(int id)
        {
          return (_context.Lecturer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
