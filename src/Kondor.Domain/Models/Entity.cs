using System.ComponentModel.DataAnnotations.Schema;
using Kondor.Domain.Enums;

namespace Kondor.Domain.Models
{
    public class Entity
    {
        public Entity()
        {
            // default value for row status
            RowStatus = RowStatus.NotRemoved;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public RowStatus RowStatus { get; set; }
    }
}
