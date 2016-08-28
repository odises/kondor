using System.Collections.Generic;

namespace YourDictionary.Worker.ApiModels
{
    public class Keyboard
    {
        public string text { get; set; }
    }

    public class ReplyMarkupModel
    {
        public List<List<Keyboard>> keyboard { get; set; }
    }
}
