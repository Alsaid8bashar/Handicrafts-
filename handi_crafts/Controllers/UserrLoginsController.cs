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
    public class UserrLoginsController : Controller
    {
        private readonly ModelContext _context;

        public UserrLoginsController(ModelContext context)
        {
            _context = context;
        }

        // GET: UserrLogins
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.UserrLogins.Include(u => u.Role).Include(u => u.Userr);
            return View(await modelContext.ToListAsync());
        }

        // GET: UserrLogins/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrLogin = await _context.UserrLogins
                .Include(u => u.Role)
                .Include(u => u.Userr)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userrLogin == null)
            {
                return NotFound();
            }

            return View(userrLogin);
        }

        // GET: UserrLogins/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id");
            return View();
        }

        // POST: UserrLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserrName,Passwordd,RoleId,UserrId,Email")] UserrLogin userrLogin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userrLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userrLogin.RoleId);
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id", userrLogin.UserrId);
            return View(userrLogin);
        }

        // GET: UserrLogins/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrLogin = await _context.UserrLogins.FindAsync(id);
            if (userrLogin == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userrLogin.RoleId);
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id", userrLogin.UserrId);
            return View(userrLogin);
        }

        // POST: UserrLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,UserrName,Passwordd,RoleId,UserrId,Email")] UserrLogin userrLogin)
        {
            if (id != userrLogin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userrLogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserrLoginExists(userrLogin.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userrLogin.RoleId);
            ViewData["UserrId"] = new SelectList(_context.Userrs, "Id", "Id", userrLogin.UserrId);
            return View(userrLogin);
        }

        // GET: UserrLogins/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userrLogin = await _context.UserrLogins
                .Include(u => u.Role)
                .Include(u => u.Userr)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userrLogin == null)
            {
                return NotFound();
            }

            return View(userrLogin);
        }

        // POST: UserrLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var userrLogin = await _context.UserrLogins.FindAsync(id);
            _context.UserrLogins.Remove(userrLogin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserrLoginExists(decimal id)
        {
            return _context.UserrLogins.Any(e => e.Id == id);
        }
    }
}
