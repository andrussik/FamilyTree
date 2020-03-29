using System.Collections.Generic;
using System.ComponentModel;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels
{
    public class PersonsViewModel
    {
        public Person Person { get; set; }
        [DisplayName("Picture")]
        public IFormFile? File { get; set; }
        public int? FamilyTreeId { get; set; }
        public List<Person> Persons { get; set; }
        public SelectList GenderSelectList { get; set; }
    }
}