using System.Collections.Generic;

namespace YourDictionary.Worker.ApiModels
{
    public class From
    {
        public int id { get; set; } 
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
        public string title { get; set; }
    }

    public class NewChatParticipant
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class NewChatMember
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class TelegramMessage
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public NewChatParticipant new_chat_participant { get; set; }
        public NewChatMember new_chat_member { get; set; }
    }

    public class Result
    {
        public int update_id { get; set; }
        public TelegramMessage message { get; set; }
    }

    public class GetUpdatesResult
    {
        public bool ok { get; set; }
        public List<Result> result { get; set; }
    }
}
