namespace Kondor.Domain.Models
{
    public class ExampleView : Entity
    {
        public int ExampleId { get; set; }
        public string UserId { get; set; }
        public int Views { get; set; }
        public virtual Example Example { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
