using System;
using Kondor.Data.Enums;

namespace Kondor.Data.DataModel
{
    public class Card : Entity
    {
        public Position CardPosition { get; set; }
        public int MemId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ExaminationDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string UserId { get; set; }
        public CardStatus Status { get; set; }
        public virtual Mem Mem { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
