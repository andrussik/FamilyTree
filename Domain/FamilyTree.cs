using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Identity;

namespace Domain
{
    public class FamilyTree
    {
        public int FamilyTreeId { get; set; }

        [DisplayName("Family tree name")] 
        [MinLength(1)]
        [MaxLength(50)]
        public string FamilyTreeName { get; set; } = default!;

        public Boolean IsPublic { get; set; } = default!;

        public int UserId { get; set; } = default!;
        public AppUser? User { get; set; }

        public ICollection<Person>? Persons { get; set; }
    }
}