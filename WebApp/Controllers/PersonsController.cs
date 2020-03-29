using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class PersonsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PersonsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
        public async Task<IActionResult> Create(int id, Person person, IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                person.FamilyTreeId = id;

                if (file != null)
                {
                    var fileName = UploadPicture(file);
                    person.Picture = fileName;
                }
                else if (person.GenderId == 1)
                {
                    person.Picture = "female-user-avatar.png";
                } 
                else if (person.GenderId == 2)
                {
                    person.Picture = "male-user-avatar.png";
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
        public async Task<IActionResult> Edit(int id, Person person, IFormFile? file)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var fileName = UploadPicture(file);
                    DeletePicture(person.Picture);
                    person.Picture = fileName;
                }
                
                _context.Persons.Update(person);
                await _context.SaveChangesAsync();

                return RedirectToAction("PeopleInFamilyTree", "FamilyTrees", new { id = person.FamilyTreeId });
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
            var familyTreeId = person.FamilyTreeId;
            DeletePicture(person.Picture);
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction("PeopleInFamilyTree", "FamilyTrees", new { id = familyTreeId });
        }
        
        private string? UploadPicture(IFormFile file)  
        {
            string uniqueFileName = null; 
            
            if (file.Length > 0 && new[] {".pdf", ".jpg", ".png", "jpeg"}
                .Any(x => x == Path.GetExtension(file.FileName)))
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        private void DeletePicture(string fileName)
        {
            if (fileName == "female-user-avatar.png" || fileName == "male-user-avatar.png") return;
            
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            var filePath = Path.Combine(uploadsFolder, fileName);
            
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
