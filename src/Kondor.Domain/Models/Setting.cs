using System;

namespace Kondor.Domain.Models
{
    public class Setting : Entity
    {
        public DateTime CreationDate { get; set; }
        public string SettingType { get; set; }
        public string SettingData { get; set; }
    }
}
