using Newtonsoft.Json;

namespace Kondor.Data.TelegramTypes
{
    public class Message
    {
        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("from")]
        public User From { get; set; }

        [JsonProperty("date")]
        public int Date { get; set; }

        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("forward_from")]
        public User ForwardFrom { get; set; }

        [JsonProperty("forward_from_chat")]
        public Chat ForwardFromChat { get; set; }

        [JsonProperty("forward_date")]
        public int ForwardDate { get; set; }

        [JsonProperty("reply_to_message")]
        public Message ReplyToMessage { get; set; }

        [JsonProperty("edit_date")]
        public int EditDate { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("entities")]
        public MessageEntity[] Entities { get; set; }

        [JsonProperty("audio")]
        public Audio Audio { get; set; }

        [JsonProperty("document")]
        public Document Document { get; set; }

        [JsonProperty("game")]
        public Game Game { get; set; }

        [JsonProperty("photo")]
        public PhotoSize[] Photo { get; set; }

        [JsonProperty("sticker")]
        public Sticker Sticker { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }

        [JsonProperty("voice")]
        public Voice Voice { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("contact")]
        public Contact Contact { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("venue")]
        public Venue Venue { get; set; }

        [JsonProperty("new_chat_member")]
        public User NewChatMember { get; set; }

        [JsonProperty("left_chat_member")]
        public User LeftChatMember { get; set; }

        [JsonProperty("new_chat_title")]
        public string NewChatTitle { get; set; }

        [JsonProperty("new_chat_photo")]
        public PhotoSize[] NewChatPhoto { get; set; }

        [JsonProperty("delete_chat_photo")]
        public bool DeleteChatPhoto => true;

        [JsonProperty("group_chat_created")]
        public bool GroupChatCreated => true;

        [JsonProperty("supergroup_chat_created")]
        public bool SuperGroupChatCreated => true;

        [JsonProperty("channel_chat_created")]
        public bool ChannelChatCreated => true;

        [JsonProperty("migrate_to_chat_id")]
        public int MigrateToChatId { get; set; }

        [JsonProperty("migrate_from_chat_id")]
        public int MigrateFromChatId { get; set; }

        [JsonProperty("pinned_message")]
        public Message PinnedMessage { get; set; }
    }
}
