using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class AppUser : IdentityUser<int>
    {
        [MinLength(1)]
        [MaxLength(256)]
        public string FirstName { get; set; } = default!;

        [MinLength(1)]
        [MaxLength(256)]
        public string LastName { get; set; } = default!;

        public ICollection<FamilyTree>? FamilyTrees { get; set; }
    }
}