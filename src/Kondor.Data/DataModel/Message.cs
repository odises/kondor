namespace Kondor.Data.DataModel
{
    public class Message : Entity
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int UpdateId { get; set; }
        public string SerializedResult { get; set; }
        public string MessageText { get; set; }
        public int Status { get; set; }
        public int ChatId { get; set; }
        public string ChatType { get; set; }
    }
}
