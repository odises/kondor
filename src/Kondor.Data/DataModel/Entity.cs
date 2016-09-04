using System.ComponentModel.DataAnnotations.Schema;

namespace Kondor.Data.DataModel
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
