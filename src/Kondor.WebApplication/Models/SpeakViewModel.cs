namespace Kondor.WebApplication.Models
{
    public class SpeakViewModel
    {
        public string Text { get; set; }
        public byte[] VoiceData { get; set; }
        public string ContentType { get; set; }
    }
}