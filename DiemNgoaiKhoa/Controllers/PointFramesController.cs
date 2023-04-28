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
    public class PointFramesController : Controller
    {
        private readonly DataContext _context;

        public PointFramesController(DataContext context)
        {
            _context = context;
        }

        // GET: PointFrames
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.PointFrames.Include(p => p.ItemDetail);
            return View(await dataContext.ToListAsync());
        }

        // GET: PointFrames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PointFrames == null)
            {
                return NotFound();
            }

            var pointFrame = await _context.PointFrames
                .Include(p => p.ItemDetail)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointFrame == null)
            {
                return NotFound();
            }

            return View(pointFrame);
        }

        // GET: PointFrames/Create
        public IActionResult Create()
        {
            ViewData["ItemDetailId"] = new SelectList(_context.ItemDetails, "Id", "Name");
            return View();
        }

        // POST: PointFrames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PointFrameRequest request)
        {
            PointFrame pointFrame = new PointFrame();
            if (ModelState.IsValid)
            {
                pointFrame.ItemDetailId = request.ItemDetailId;
                pointFrame.Name = request.Name;
                pointFrame.MaxPoint = request.MaxPoint;
                _context.Add(pointFrame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemDetailId"] = new SelectList(_context.ItemDetails, "Id", "Name", pointFrame.ItemDetailId);
            return View(pointFrame);
        }

        // GET: PointFrames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PointFrames == null)
            {
                return NotFound();
            }

            var pointFrame = await _context.PointFrames.FindAsync(id);
            if (pointFrame == null)
            {
                return NotFound();
            }
            ViewData["ItemDetailId"] = new SelectList(_context.ItemDetails, "Id", "Name", pointFrame.ItemDetailId);
            return View(pointFrame);
        }

        public async Task<PointFrame> GetById(int id)
        {
            return await this._context.PointFrames.Where(c=>c.Id == id).FirstAsync();
        }

        // POST: PointFrames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PointFrameRequest request)
        {
            PointFrame pointFrame = await this.GetById(id);
            if (id != pointFrame.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pointFrame.ItemDetailId = request.ItemDetailId;
                    pointFrame.Name = request.Name;
                    pointFrame.MaxPoint = request.MaxPoint;
                    _context.Update(pointFrame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PointFrameExists(pointFrame.Id))
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
            ViewData["ItemDetailId"] = new SelectList(_context.ItemDetails, "Id", "Name", pointFrame.ItemDetailId);
            return View(pointFrame);
        }

        // GET: PointFrames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PointFrames == null)
            {
                return NotFound();
            }

            var pointFrame = await _context.PointFrames
                .Include(p => p.ItemDetail)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pointFrame == null)
            {
                return NotFound();
            }

            return View(pointFrame);
        }

        // POST: PointFrames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PointFrames == null)
            {
                return Problem("Entity set 'DataContext.PointFrames'  is null.");
            }
            var pointFrame = await _context.PointFrames.FindAsync(id);
            if (pointFrame != null)
            {
                _context.PointFrames.Remove(pointFrame);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PointFrameExists(int id)
        {
          return (_context.PointFrames?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
