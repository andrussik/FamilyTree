using System.Collections.Generic;

namespace Domain
{
    public class PersonFamilyTree
    {
        public int PersonFamilyTreeId { get; set; }

        public ICollection<Person>? Persons { get; set; }

        public ICollection<FamilyTree>? FamilyTrees { get; set; }
    }
}