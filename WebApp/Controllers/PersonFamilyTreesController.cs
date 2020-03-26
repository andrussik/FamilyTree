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
    public class PersonFamilyTreesController : Controller
    {
        private readonly AppDbContext _context;

        public PersonFamilyTreesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PersonFamilyTrees
        public async Task<IActionResult> Index()
        {
            return View(await _context.PersonFamilyTrees.ToListAsync());
        }

        // GET: PersonFamilyTrees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personFamilyTree = await _context.PersonFamilyTrees
                .FirstOrDefaultAsync(m => m.PersonFamilyTreeId == id);
            if (personFamilyTree == null)
            {
                return NotFound();
            }

            return View(personFamilyTree);
        }

        // GET: PersonFamilyTrees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PersonFamilyTrees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonFamilyTreeId")] PersonFamilyTree personFamilyTree)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personFamilyTree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personFamilyTree);
        }

        // GET: PersonFamilyTrees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personFamilyTree = await _context.PersonFamilyTrees.FindAsync(id);
            if (personFamilyTree == null)
            {
                return NotFound();
            }
            return View(personFamilyTree);
        }

        // POST: PersonFamilyTrees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonFamilyTreeId")] PersonFamilyTree personFamilyTree)
        {
            if (id != personFamilyTree.PersonFamilyTreeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personFamilyTree);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonFamilyTreeExists(personFamilyTree.PersonFamilyTreeId))
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
            return View(personFamilyTree);
        }

        // GET: PersonFamilyTrees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personFamilyTree = await _context.PersonFamilyTrees
                .FirstOrDefaultAsync(m => m.PersonFamilyTreeId == id);
            if (personFamilyTree == null)
            {
                return NotFound();
            }

            return View(personFamilyTree);
        }

        // POST: PersonFamilyTrees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personFamilyTree = await _context.PersonFamilyTrees.FindAsync(id);
            _context.PersonFamilyTrees.Remove(personFamilyTree);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonFamilyTreeExists(int id)
        {
            return _context.PersonFamilyTrees.Any(e => e.PersonFamilyTreeId == id);
        }
    }
}
