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
    public class PersonsController : Controller
    {
        private readonly AppDbContext _context;

        public PersonsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Persons
        public async Task<IActionResult> Index()
        {
            var vm = new PersonsViewModel
            {
                Persons = await _context.Persons
                    .Include(p => p.Gender)
                    .Include(p => p.FamilyTree)
                    .ToListAsync()
            };
            
            return View(vm);
        }

        // GET: Persons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var vm = new PersonsViewModel
            {
                Person = await _context.Persons
                    .Include(p => p.Gender)
                    .Include(p => p.FamilyTree)
                    .FirstOrDefaultAsync(m => m.PersonId == id)
            };

            
            if (vm.Person == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // GET: Persons/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = new PersonsViewModel
            {
                FamilyTreeId = id,
                GenderSelectList = new SelectList(_context.Genders, "GenderId", "Name"),
            };
            
            return View(vm);
        }

        // POST: Persons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, Person person)
        {
            
            if (ModelState.IsValid)
            {
                person.FamilyTreeId = id;
                if (person.ImageSource == null && person.GenderId == 1)
                {
                    person.ImageSource = "~/images/female-user-avatar.png";
                } 
                else if (person.ImageSource == null && person.GenderId == 2)
                {
                    person.ImageSource = "~/images/male-user-avatar.png";
                }
                
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction("PeopleInFamilyTree", "FamilyTrees", new { id });
            }
            
            var vm = new PersonsViewModel
            {
                GenderSelectList = new SelectList(_context.Genders, "GenderId", "Name", person.GenderId)
            };
            
            return View(vm);
        }

        // GET: Persons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            
            if (person == null)
            {
                return NotFound();
            }
            
            var vm = new PersonsViewModel
            {
                Person = person,
                GenderSelectList = new SelectList(_context.Genders, "GenderId", "Name", person.GenderId)
            };

            return View(vm);
        }

        // POST: Persons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                _context.Update(person);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("PeopleInFamilyTree", "FamilyTrees", new { id });
            }
            var vm = new PersonsViewModel
            {
                Person = person,
                GenderSelectList = new SelectList(_context.Genders, "GenderId", "Name", person.GenderId)
            };
            
            return View(vm);
        }

        // GET: Persons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.Gender)
                .Include(p => p.FamilyTree)
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            var vm = new PersonsViewModel
            {
                Person = person
            };
                
            return View(vm);
        }

        // POST: Persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction("PeopleInFamilyTree", "FamilyTrees", new { id });
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.PersonId == id);
        }
    }
}
