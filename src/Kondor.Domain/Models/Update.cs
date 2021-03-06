﻿using System;
using Kondor.Domain.Enums;

namespace Kondor.Domain.Models
{
    public class Update : Entity
    {
        public int UpdateId { get; set; }
        public int FromId { get; set; }
        public UpdateType UpdateType { get; set; }
        public string SerializedUpdate { get; set; }
        public DateTime CreationDatetime { get; set; }
        public UpdateStatus Status { get; set; }
        public DateTime ModifiedDatetime { get; set; }
    }
}
