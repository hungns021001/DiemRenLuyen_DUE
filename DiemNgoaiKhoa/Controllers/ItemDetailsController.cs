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
    public class ItemDetailsController : Controller
    {
        private readonly DataContext _context;

        public ItemDetailsController(DataContext context)
        {
            _context = context;
        }

        // GET: ItemDetails
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.ItemDetails.Include(i => i.Item);
            return View(await dataContext.ToListAsync());
        }

        // GET: ItemDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ItemDetails == null)
            {
                return NotFound();
            }

            var itemDetail = await _context.ItemDetails
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemDetail == null)
            {
                return NotFound();
            }

            return View(itemDetail);
        }

        // GET: ItemDetails/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name");
            return View();
        }

        // POST: ItemDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemDetailRequest request)
        {
            ItemDetail itemDetail = new ItemDetail();
            if (ModelState.IsValid)
            {
                itemDetail.Name = request.Name;
                itemDetail.ItemId = request.ItemId;
                _context.Add(itemDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", itemDetail.ItemId);
            return View(itemDetail);
        }

        // GET: ItemDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ItemDetails == null)
            {
                return NotFound();
            }

            var itemDetail = await _context.ItemDetails.FindAsync(id);
            if (itemDetail == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", itemDetail.ItemId);
            return View(itemDetail);
        }

        public async Task<ItemDetail> GetById (int id)
        {
            return await this._context.ItemDetails.Where(c => c.Id == id).FirstAsync();
        }

        // POST: ItemDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ItemDetailRequest request)
        {
            ItemDetail itemDetail = await this.GetById(id);
            if (id != itemDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    itemDetail.Name = request.Name;
                    itemDetail.ItemId = request.ItemId;
                    _context.Update(itemDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemDetailExists(itemDetail.Id))
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
            ViewData["ItemId"] = new SelectList(_context.Items, "Id", "Name", itemDetail.ItemId);
            return View(itemDetail);
        }

        // GET: ItemDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ItemDetails == null)
            {
                return NotFound();
            }

            var itemDetail = await _context.ItemDetails
                .Include(i => i.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemDetail == null)
            {
                return NotFound();
            }

            return View(itemDetail);
        }

        // POST: ItemDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ItemDetails == null)
            {
                return Problem("Entity set 'DataContext.ItemDetails'  is null.");
            }
            var itemDetail = await _context.ItemDetails.FindAsync(id);
            if (itemDetail != null)
            {
                _context.ItemDetails.Remove(itemDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemDetailExists(int id)
        {
          return (_context.ItemDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
