namespace ImageDownloader.Core.Messages
{
    public class OutputMessage
    {
        public string Text { get; set; }

        public OutputMessage(string text)
        {
            Text = text;
        }
    }
}
