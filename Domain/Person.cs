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

        [DisplayName("First name")]
        [MinLength(1)]
        [MaxLength(150)]
        public string LastName { get; set; } = default!;

        public Gender Gender { get; set; } = default!;

        [DisplayName("Date of birth")] 
        public DateTime DateOfBirth { get; set; } = default!;

        [InverseProperty("Child")]
        public ICollection<Relationship>? ChildRelationships { get; set; }
        
        [InverseProperty("Parent")]
        public ICollection<Relationship>? ParentRelationships { get; set; }
    }

    public enum Gender
    {
        Female,
        Male,
        Unknown
    }
}