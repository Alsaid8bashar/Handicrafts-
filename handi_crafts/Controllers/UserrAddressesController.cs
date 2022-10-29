using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using handi_crafts.Models;

namespace handi_crafts.Controllers
{
    public class UserrAddressesController : Controller
    {
        private readonly ModelContext _context;

        public UserrAddressesController(ModelContext context)
        {
            _context = context;
        }

        // GET: UserrAddresses
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.UserrAddresses.Include(u => u.Userr);
            return View(await modelContext.ToListAsync());
        }

        // GET: UserrAddresses/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrAddress = await _context.UserrAddresses
                .Include(u => u.Userr)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userrAddress == null)
            {
                return NotFound();
            }

            return View(userrAddress);
        }

        // GET: UserrAddresses/Create
        public IActionResult Create()
        {
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id");
            return View();
        }

        // POST: UserrAddresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,AddressLine1,AddressLine2,City,Country,PhoneNumber,ZipCode,UserrId")] UserrAddress userrAddress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userrAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id", userrAddress.UserrId);
            return View(userrAddress);
        }

        // GET: UserrAddresses/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrAddress = await _context.UserrAddresses.FindAsync(id);
            if (userrAddress == null)
            {
                return NotFound();
            }
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id", userrAddress.UserrId);
            return View(userrAddress);
        }

        // POST: UserrAddresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,FirstName,LastName,AddressLine1,AddressLine2,City,Country,PhoneNumber,ZipCode,UserrId")] UserrAddress userrAddress)
        {
            if (id != userrAddress.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userrAddress);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserrAddressExists(userrAddress.Id))
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
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id", userrAddress.UserrId);
            return View(userrAddress);
        }

        // GET: UserrAddresses/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrAddress = await _context.UserrAddresses
                .Include(u => u.Userr)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userrAddress == null)
            {
                return NotFound();
            }

            return View(userrAddress);
        }

        // POST: UserrAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var userrAddress = await _context.UserrAddresses.FindAsync(id);
            _context.UserrAddresses.Remove(userrAddress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserrAddressExists(decimal id)
        {
            return _context.UserrAddresses.Any(e => e.Id == id);
        }
    }
}
