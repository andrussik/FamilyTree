using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
using WebApp.ViewModels;

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
            var appDbContext = _context.Relationships.Include(r => r.Child).Include(r => r.Parent).Include(r => r.RelationshipType);
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
                .Include(r => r.RelationshipType)
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
            ViewData["RelationshipTypeId"] = new SelectList(_context.RelationshipTypes, "RelationshipTypeId", "Type");
            return View();
        }

        // POST: Relationships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RelationshipId,RelationshipTypeId,ChildId,ParentId")] Relationship relationship)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relationship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChildId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ChildId);
            ViewData["ParentId"] = new SelectList(_context.Persons, "PersonId", "FirstName", relationship.ParentId);
            ViewData["RelationshipTypeId"] = new SelectList(_context.RelationshipTypes, "RelationshipTypeId", "Type", relationship.RelationshipTypeId);
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
            ViewData["RelationshipTypeId"] = new SelectList(_context.RelationshipTypes, "RelationshipTypeId", "Type", relationship.RelationshipTypeId);
            return View(relationship);
        }

        // POST: Relationships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RelationshipId,RelationshipTypeId,ChildId,ParentId")] Relationship relationship)
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
            ViewData["RelationshipTypeId"] = new SelectList(_context.RelationshipTypes, "RelationshipTypeId", "Type", relationship.RelationshipTypeId);
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
                .Include(r => r.RelationshipType)
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
        
        // GET: RelationshipTypes/Create
        public async Task<IActionResult> EditParents(int childId)
        {
            var child = await  _context.Persons
                .Include(p => p.ParentRelationships)
                .FirstOrDefaultAsync(p => p.PersonId == childId);

            var childRelationships = await _context.Relationships
                .Include(r => r.Parent)
                .ThenInclude(p => p.Gender)
                .Where(r => r.ChildId == childId).ToListAsync();
            
            int? motherId = null;
            int? fatherId = null;
            if (childRelationships.Any())
            {
                foreach (var relationship in childRelationships)
                {
                    if (relationship.Parent.Gender.Name == "female")
                    {
                        motherId = relationship.Parent.PersonId;
                    }
                    else if (relationship.Parent.Gender.Name == "male")
                    {
                        fatherId = relationship.Parent.PersonId;
                    }
                }
            }
            
            var vm = new EditParentsViewModel()
            {
                Child = child,
                MotherSelectList = new SelectList(_context.Persons
                    .Where(g => g.DateOfBirth < child.DateOfBirth)
                    .Where(g => g.Gender.Name == "female"),
                    "PersonId", "FirstLastName", motherId),
                FatherSelectList = new SelectList(_context.Persons
                    .Where(g => g.DateOfBirth < child.DateOfBirth)
                    .Where(g => g.Gender.Name == "male"),
                    "PersonId", "FirstLastName", fatherId)
            };

            return View(vm);
        }
        
        // POST: RelationshipTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditParents(int childId, int? motherId, int? fatherId)
        {
            var childRelationships = await _context.Relationships
                .Include(r => r.Parent)
                .ThenInclude(p => p.Gender)
                .Where(r => r.ChildId == childId).ToListAsync();
            
            Person child = await _context.Persons.FirstOrDefaultAsync(p => p.PersonId == childId);
            Person currentMother = null;
            Person currentFather = null;
            
            if (childRelationships.Any())
            {
                foreach (var relationship in childRelationships)
                {
                    if (relationship.Parent.Gender.Name == "female")
                    {
                        currentMother = relationship.Parent;
                    }
                    else if (relationship.Parent.Gender.Name == "male")
                    {
                        currentFather = relationship.Parent;
                    }
                }
            }

            var motherRelationshipType = await _context.RelationshipTypes
                .FirstOrDefaultAsync(p => p.Type == "mother-child");
            var fatherRelationshipType = await _context.RelationshipTypes
                .FirstOrDefaultAsync(p => p.Type == "father-child");
            
            if (motherId != null)
            {
                var mother = await _context.Persons
                    .FirstOrDefaultAsync(p => p.PersonId == motherId);
                if (currentMother == null && mother != null)
                {
                    _context.Relationships.Add(new Relationship()
                    {
                        RelationshipTypeId = motherRelationshipType.RelationshipTypeId,
                        ParentId = (int) motherId,
                        ChildId = childId
                    });
                }
                else if (currentMother != null && mother != null && currentMother.PersonId != motherId)
                {
                    var relationship = await _context.Relationships
                        .FirstOrDefaultAsync(a => a.ChildId == childId && 
                                                  a.ParentId == currentMother.PersonId);
                    if (relationship != null)
                    {                    
                        _context.Relationships.Remove(relationship);
                    }
                    
                    _context.Relationships.Add(new Relationship()
                    {
                        RelationshipTypeId = motherRelationshipType.RelationshipTypeId,
                        ParentId = (int) motherId,
                        ChildId = childId
                    });
                }
            }
            
            if (fatherId != null)
            {
                var father = await _context.Persons
                    .FirstOrDefaultAsync(p => p.PersonId == fatherId);
                if (currentFather == null && father != null)
                {
                    _context.Relationships.Add(new Relationship()
                    {
                        RelationshipTypeId = fatherRelationshipType.RelationshipTypeId,
                        ParentId = (int) fatherId,
                        ChildId = childId
                    });
                }
                else if (currentFather != null && father != null && currentFather.PersonId != fatherId)
                {
                    var relationship = await _context.Relationships
                        .FirstOrDefaultAsync(a => a.ChildId == childId && 
                                             a.ParentId == currentFather.PersonId);
                    if (relationship != null)
                    {                    
                        _context.Relationships.Remove(relationship);
                    }
                    
                    _context.Relationships.Add(new Relationship()
                    {
                        RelationshipTypeId = fatherRelationshipType.RelationshipTypeId,
                        ParentId = (int) fatherId,
                        ChildId = childId
                    });
                }
            }

            await _context.SaveChangesAsync();

            // return View();
            return RedirectToAction("PeopleInFamilyTree", "FamilyTrees", new { id = child.FamilyTreeId });   
        }

        private bool RelationshipExists(int id)
        {
            return _context.Relationships.Any(e => e.RelationshipId == id);
        }
    }
}
