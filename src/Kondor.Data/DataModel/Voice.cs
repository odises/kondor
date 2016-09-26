namespace Kondor.Data.DataModel
{
    public class Voice : Entity
    {
        public string Text { get; set; }
        public byte[] VoiceData { get; set; }
        public string ContentType { get; set; }
    }
}
