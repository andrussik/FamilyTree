using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;

namespace WebApp.Controllers
{
    public class FamilyTreesController : Controller
    {
        private readonly AppDbContext _context;

        public FamilyTreesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: FamilyTrees
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.FamilyTrees.Include(f => f.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: FamilyTrees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familyTree = await _context.FamilyTrees
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FamilyTreeId == id);
            if (familyTree == null)
            {
                return NotFound();
            }

            return View(familyTree);
        }

        // GET: FamilyTrees/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName");
            return View();
        }

        // POST: FamilyTrees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FamilyTreeId,FamilyTreeName,IsPublic,UserId")] FamilyTree familyTree)
        {
            if (ModelState.IsValid)
            {
                _context.Add(familyTree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", familyTree.UserId);
            return View(familyTree);
        }

        // GET: FamilyTrees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familyTree = await _context.FamilyTrees.FindAsync(id);
            if (familyTree == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", familyTree.UserId);
            return View(familyTree);
        }

        // POST: FamilyTrees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FamilyTreeId,FamilyTreeName,IsPublic,UserId")] FamilyTree familyTree)
        {
            if (id != familyTree.FamilyTreeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(familyTree);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FamilyTreeExists(familyTree.FamilyTreeId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", familyTree.UserId);
            return View(familyTree);
        }

        // GET: FamilyTrees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familyTree = await _context.FamilyTrees
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FamilyTreeId == id);
            if (familyTree == null)
            {
                return NotFound();
            }

            return View(familyTree);
        }

        // POST: FamilyTrees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var familyTree = await _context.FamilyTrees.FindAsync(id);
            _context.FamilyTrees.Remove(familyTree);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FamilyTreeExists(int id)
        {
            return _context.FamilyTrees.Any(e => e.FamilyTreeId == id);
        }
    }
}
