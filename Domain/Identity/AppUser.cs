using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        [DisplayName("Date of birth")] 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = default!;
        
        public string? ImageSource { get; set; }
        
        public int GenderId { get; set; } = default!;
        public Gender? Gender { get; set; }

        public ICollection<FamilyTree>? FamilyTrees { get; set; }
    }
}