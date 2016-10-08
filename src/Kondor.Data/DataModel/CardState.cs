using System;
using Kondor.Data.Enums;

namespace Kondor.Data.DataModel
{
    public class CardState : Entity
    {
        public Position CardPosition { get; set; }
        public int CardId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ExaminationDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string UserId { get; set; }
        public InboxCardsStatus Status { get; set; }
        public virtual Card Card { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
