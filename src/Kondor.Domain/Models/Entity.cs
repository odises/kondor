using System.ComponentModel.DataAnnotations.Schema;
using Kondor.Domain.Enums;

namespace Kondor.Domain.Models
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
