using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class RelationshipType
    {
        public int RelationshipTypeId { get; set; }

        [DisplayName("Relationship type")]
        [MinLength(1)]
        [MaxLength(100)]
        public string Type { get; set; } = default!;

        public ICollection<Relationship>? Relationships { get; set; }
    }
}