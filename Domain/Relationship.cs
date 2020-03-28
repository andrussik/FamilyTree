using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Relationship
    {
        public int RelationshipId { get; set; }

        public int RelationshipTypeId { get; set; } = default!;
        public RelationshipType? RelationshipType { get; set; }

        public int ChildId { get; set; } = default!;
        public Person? Child { get; set; }

        public int ParentId { get; set; } = default!;
        public Person? Parent { get; set; }
    }
}