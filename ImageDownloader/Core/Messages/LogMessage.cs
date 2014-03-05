namespace ImageDownloader.Core.Messages
{
    public class LogMessage
    {
        public string Text { get; set; }

        public LogMessage(string text)
        {
            Text = text;
        }
    }
}
