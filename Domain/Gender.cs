using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Gender
    {
        public int GenderId { get; set; }
        
        [DisplayName("Gender")]
        [MinLength(1)]
        [MaxLength(20)]
        public string Name { get; set; } = default!;

        public ICollection<Person>? Persons { get; set; }
    }
}