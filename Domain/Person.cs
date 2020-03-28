using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Person
    {
        public int PersonId { get; set; }

        [DisplayName("First name")]
        [MinLength(1)]
        [MaxLength(150)]
        public string FirstName { get; set; } = default!;

        [DisplayName("Last name")]
        [MinLength(1)]
        [MaxLength(150)]
        public string LastName { get; set; } = default!;

        [DisplayName("Date of birth")] 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = default!;

        public string? ImageSource { get; set; }
        
        [DisplayName("Gender")]
        public int GenderId { get; set; } = default!;
        public Gender? Gender { get; set; }

        public int FamilyTreeId { get; set; } = default!;
        public FamilyTree? FamilyTree { get; set; }

        [InverseProperty("Child")]
        public ICollection<Relationship>? ChildRelationships { get; set; }
        
        [InverseProperty("Parent")]
        public ICollection<Relationship>? ParentRelationships { get; set; }

        public string FirstLastName => FirstName + " " + LastName;
    }
}