using System;
using System.Collections.Generic;

namespace Kondor.Data.DataModel
{
    public class Language
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<StringResource> LanguageStringResources { get; set; }
        public virtual ICollection<ApplicationUser> LanguageUsers { get; set; }
    }
}
