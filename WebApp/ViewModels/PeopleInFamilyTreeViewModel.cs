using System.Collections.Generic;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ViewModels
{
    public class PeopleInFamilyTreeViewModel
    {
        public FamilyTree FamilyTree { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        public string LastNameSort { get; set; }
        public string MotherNameSort { get; set; }
        public string FatherNameSort { get; set; }
        public string DateOfBirthSort { get; set; }
        public string GenderSort { get; set; }
        public string BirthOrderSort { get; set; }
        public IList<Person> Persons { get; set; }
    }
}