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
    public class RelationshipsController : Controller
    {
        private readonly AppDbContext _context;

        public RelationshipsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Relationships
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Relationships.Include(r => r.Child).Include(r => r.Parent);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Relationships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationship = await _context.Relationships
                .Include(r => r.Child)
                .Include(r => r.Parent)
                .FirstOrDefaultAsync(m => m.RelationshipId == id);
            if (relationship == null)
            {
                return NotFound();
            }

            return View(relationship);
        }

        // GET: Relationships/Create
        public IActionResult Create()
        {
            ViewData["ChildId"] = new SelectList(_context.Persons, "PersonId", "FirstName");
            ViewData["ParentId"] = new SelectList(_context.Persons, "PersonId", "FirstName");
            return View();
        }

        // POST: Relationships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RelationshipId,RelationshipType,ChildId,ParentId")] Relationship relationship)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relationship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChildId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ChildId);
            ViewData["ParentId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ParentId);
            return View(relationship);
        }

        // GET: Relationships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationship = await _context.Relationships.FindAsync(id);
            if (relationship == null)
            {
                return NotFound();
            }
            ViewData["ChildId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ChildId);
            ViewData["ParentId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ParentId);
            return View(relationship);
        }

        // POST: Relationships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RelationshipId,RelationshipType,ChildId,ParentId")] Relationship relationship)
        {
            if (id != relationship.RelationshipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relationship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelationshipExists(relationship.RelationshipId))
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
            ViewData["ChildId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ChildId);
            ViewData["ParentId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ParentId);
            return View(relationship);
        }

        // GET: Relationships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relationship = await _context.Relationships
                .Include(r => r.Child)
                .Include(r => r.Parent)
                .FirstOrDefaultAsync(m => m.RelationshipId == id);
            if (relationship == null)
            {
                return NotFound();
            }

            return View(relationship);
        }

        // POST: Relationships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relationship = await _context.Relationships.FindAsync(id);
            _context.Relationships.Remove(relationship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelationshipExists(int id)
        {
            return _context.Relationships.Any(e => e.RelationshipId == id);
        }
    }
}
