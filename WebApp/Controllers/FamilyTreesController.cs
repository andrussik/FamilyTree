using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
using Microsoft.AspNetCore.Identity;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class FamilyTreesController : Controller
    {
        private readonly AppDbContext _context;

        public FamilyTreesController(AppDbContext context)
        {
            _context = context;
        }
        
        // [BindProperty(SupportsGet = true)]
        // public string SearchString { get; set; }
        // public string FirstNameSort { get; set; }
        // public string LastNameSort { get; set; }
        // public string DateOfBirthSort { get; set; }
        // public string GenderSort { get; set; }
        // public IList<Person> Persons { get; set; }

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
            familyTree.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (ModelState.IsValid)
            {
                _context.Add(familyTree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // ViewData["UserId"] = new SelectList(_context.Users, "Id", "FirstName", familyTree.UserId);
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
            familyTree.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
            familyTree.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        
        // GET: FamilyTrees/PeopleInFamilyTree
        public async Task<IActionResult> PeopleInFamilyTree(int? id, string sortOrder, string searchString)
        {
            var vm = new PeopleInFamilyTreeViewModel();
            
            vm.FamilyTree = await _context.FamilyTrees
                .Include(f => f.Persons)
                .ThenInclude(t => t.Gender)
                .Include(f => f.Persons)
                .ThenInclude(t => t.ChildRelationships)
                .ThenInclude(t => t.Parent)
                .Include(f => f.Persons)
                .ThenInclude(t => t.ChildRelationships)
                .ThenInclude(t => t.RelationshipType)
                .FirstOrDefaultAsync(m => m.FamilyTreeId == id);
            
            vm.Persons = vm.FamilyTree.Persons.ToList();
            
            vm.FirstNameSort = String.IsNullOrEmpty(sortOrder) ? "firstName_asc" : "";
            vm.LastNameSort = String.IsNullOrEmpty(sortOrder) ? "lastName_asc" : "";
            vm.DateOfBirthSort = String.IsNullOrEmpty(sortOrder) ? "dateOfBirth_asc" : "";
            vm.GenderSort = String.IsNullOrEmpty(sortOrder) ? "gender_asc" : "";
            
            IQueryable<Person> personsIQ = from s in _context.Persons
                    .Include(a => a.Gender)
                    .Include(t => t.ChildRelationships)
                    .ThenInclude(t => t.Parent)
                    .Include(t => t.ChildRelationships)
                    .ThenInclude(t => t.RelationshipType)
                    .Where(t => t.FamilyTreeId == id)
                select s;
            
            switch (sortOrder)
            {
                case "firstName_asc":
                    personsIQ = personsIQ.OrderBy(s => s.FirstName);
                    break;
                case "lastName_asc":
                    personsIQ = personsIQ.OrderBy(s => s.LastName);
                    break;
                case "dateOfBirth_asc":
                    personsIQ = personsIQ.OrderBy(s => s.DateOfBirth);
                    break;
                case "gender_asc":
                    personsIQ = personsIQ.OrderBy(s => s.Gender.Name);
                    break;
                default:
                    personsIQ = personsIQ.OrderBy(s => s.LastName);
                    break;
            }
            
            vm.Persons = await personsIQ.ToListAsync();
            
            
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToLower().Trim();
            }
            
            var personQuery = _context.Persons
                .Include(a => a.Gender)
                .Include(t => t.ChildRelationships)
                .ThenInclude(t => t.Parent)
                .Include(t => t.ChildRelationships)
                .ThenInclude(t => t.RelationshipType)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                personQuery = personQuery.Where(s =>
                    s.FirstName.ToLower().Contains(searchString) ||
                    s.LastName.ToLower().Contains(searchString)
                );
            
                personQuery = personQuery.OrderBy(a => a.LastName);

                vm.SearchString = searchString;
                vm.Persons = await personQuery.ToListAsync();
            
            }
            
            return View(vm);
        }
    }
}
