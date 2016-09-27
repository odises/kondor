using System;

namespace Kondor.Data.DataModel
{
    public class StringResource : Entity
    {
        public string GroupCode { get; set; }
        public string Text { get; set; }
        public Guid LanguageId { get; set; }
        public virtual Language Language { get; set; }
    }
}
