using System.ComponentModel;
using Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels
{
    public class EditParentsViewModel
    {
        public SelectList MotherSelectList { get; set; }
        public SelectList FatherSelectList { get; set; }
        public Person Child { get; set; }
        public int ChildId { get; set; }
        [DisplayName("Mother")]
        public int? MotherId { get; set; }
        [DisplayName("Father")]
        public int? FatherId { get; set; }
    }
}