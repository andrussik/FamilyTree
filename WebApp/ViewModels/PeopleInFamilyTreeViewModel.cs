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
        public string FirstNameSort { get; set; }
        public string LastNameSort { get; set; }
        public string DateOfBirthSort { get; set; }
        public string GenderSort { get; set; }
        public IList<Person> Persons { get; set; }
    }
}