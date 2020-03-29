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
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class FamilyTreesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public FamilyTreesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var user = await _userManager.GetUserAsync(User);
            familyTree.UserId = user.Id;

            if (ModelState.IsValid)
            {
                _context.Add(familyTree);
                await _context.SaveChangesAsync();
                var person = new Person()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Picture = user.GenderId == 1 ? 
                        "female-user-avatar.png" : 
                        "male-user-avatar.png",
                    GenderId = user.GenderId,
                    FamilyTreeId = familyTree.FamilyTreeId
                };
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
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
            Console.WriteLine(familyTree.UserId);

            return View(familyTree);
        }

        // POST: FamilyTrees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FamilyTree familyTree)
        {
            if (id != familyTree.FamilyTreeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.FamilyTrees.Update(familyTree);
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
            
            var vm = new FamilyTreesViewModel()
            {
                FamilyTree = familyTree
            };

            return View(vm);
        }

        // POST: FamilyTrees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var familyTree = await _context.FamilyTrees
                .Include(f => f.Persons)
                .FirstOrDefaultAsync(f => f.FamilyTreeId == id);
            _context.FamilyTrees.Remove(familyTree);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FamilyTreeExists(int id)
        {
            return _context.FamilyTrees.Any(e => e.FamilyTreeId == id);
        }
        
        // GET: FamilyTrees/PeopleInFamilyTree
        public async Task<IActionResult> PeopleInFamilyTree(int id, string? sortOrder, string? searchString)
        {
            var vm = new PeopleInFamilyTreeViewModel
            {
                FamilyTree = _context.FamilyTrees.FindAsync(id).Result,
                Persons = _context.Persons
                    .Include(a => a.Gender)
                    .Include(t => t.ChildRelationships)
                    .ThenInclude(t => t.Parent)
                    .Include(t => t.ChildRelationships)
                    .ThenInclude(t => t.RelationshipType)
                    .Where(p => p.FamilyTreeId == id)
                    .ToList(),
                LastNameSort = string.IsNullOrEmpty(sortOrder) ? "lastName_asc" : "",
                MotherNameSort = string.IsNullOrEmpty(sortOrder) ? "motherName_asc" : "",
                FatherNameSort = string.IsNullOrEmpty(sortOrder) ? "fatherName_asc" : "",
                DateOfBirthSort = string.IsNullOrEmpty(sortOrder) ? "dateOfBirth_asc" : "",
                GenderSort = string.IsNullOrEmpty(sortOrder) ? "gender_asc" : "",
                BirthOrderSort = string.IsNullOrEmpty(sortOrder) ? "birthOrder_asc" : "",
            };

            if (sortOrder != null)
            {
                vm.Persons = await Sort(id, sortOrder);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToLower().Trim();
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    vm.Persons = await Search(searchString);
                    vm.SearchString = searchString;
                }
                
            }
            
            return View(vm);
        }
        
        public async Task<List<Person>> Sort(int id, string sortOrder)
        {
            IQueryable<Person> personsIQ = from s in _context.Persons
                    .Include(a => a.Gender)
                    .Include(t => t.ChildRelationships)
                    .ThenInclude(t => t.Parent)
                    .ThenInclude(t => t.ParentRelationships)
                    .ThenInclude(t => t.Child)
                    .Include(t => t.ChildRelationships)
                    .ThenInclude(t => t.RelationshipType)
                    .Where(t => t.FamilyTreeId == id)
                select s;
        
            switch (sortOrder)
            {
                case "lastName_asc":
                    personsIQ = personsIQ.OrderBy(s => s.LastName);
                    break;
                case "motherName_asc":
                    personsIQ = personsIQ.OrderBy(s => s.GetMother() != null ? s.GetMother().LastName : string.Empty);
                    break;
                case "fatherName_asc":
                    personsIQ = personsIQ.OrderBy(s => s.GetFather() != null ? s.GetFather().LastName : string.Empty);
                    break;
                case "dateOfBirth_asc":
                    personsIQ = personsIQ.OrderBy(s => s.DateOfBirth);
                    break;
                case "gender_asc":
                    personsIQ = personsIQ.OrderBy(s => s.Gender.Name);
                    break;
                case "birthOrder_asc":
                    personsIQ = personsIQ.OrderBy(s => s.GetBirthOrder() != null ? s.GetBirthOrder() : int.MaxValue);
                    break;
                default:
                    personsIQ = personsIQ.OrderBy(s => s.LastName);
                    break;
            }
            
            return await personsIQ.ToListAsync();
        }

        public async Task<List<Person>> Search(string searchString)
        {
            var personQuery = _context.Persons
                .Include(a => a.Gender)
                .Include(t => t.ChildRelationships)
                .ThenInclude(t => t.Parent)
                .Include(t => t.ChildRelationships)
                .ThenInclude(t => t.RelationshipType)
                .AsQueryable();


            personQuery = personQuery.Where(s =>
                s.FirstName.ToLower().Contains(searchString) ||
                s.LastName.ToLower().Contains(searchString)
            );

            personQuery = personQuery.OrderBy(a => a.LastName);

            return await personQuery.ToListAsync();
        }
    }
}
