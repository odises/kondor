using System;
using Kondor.Service.Enums;

namespace Kondor.Service.DataModel
{
    public class Card
    {
        public int Id { get; set; }
        public Position CardPosition { get; set; }
        public int WordId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ExaminationDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public int UserId { get; set; }
        public CardStatus Status { get; set; }
        public virtual Word Word { get; set; }
        public virtual User User { get; set; }
    }
}
