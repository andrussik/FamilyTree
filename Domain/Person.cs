using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public string? Picture { get; set; }
        
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

        public Person? GetMother()
        {
            if (ChildRelationships == null) return null;

            Person mother = null;
            
            foreach (var relationship in ChildRelationships)
            {
                if (relationship.RelationshipType.Type == "mother-child")
                {
                    mother = relationship.Parent;
                }
                
            }

            return mother;
        }
        
        public Person? GetFather()
        {
            if (ChildRelationships == null) return null;

            Person father = null;
            
            foreach (var relationship in ChildRelationships)
            {
                if (relationship.RelationshipType.Type == "father-child")
                {
                    father = relationship.Parent;
                }
                
            }

            return father;
        }

        public List<Person>? GetChildren()
        {
            if (ParentRelationships == null) return null;
            
            var children = new List<Person>();
            var relationships = ParentRelationships.ToList();
        
            foreach (var relationship in relationships)
            {
                children.Add(relationship.Child);
            }
        
            return children;
        }
        
        public int? GetBirthOrder()
        {
            var mother = GetMother();
            var father = GetFather();
            var motherChildren = mother?.GetChildren();
            var fatherChildren = father?.GetChildren();
            var birthOrder = motherChildren != null && fatherChildren != null ?
                motherChildren
                .Union(fatherChildren)
                .OrderBy(o => o.DateOfBirth)
                .ToList().FindIndex(a => a.PersonId == PersonId) + 1 :
                (int?) null;

            return birthOrder;
        }
    }
}