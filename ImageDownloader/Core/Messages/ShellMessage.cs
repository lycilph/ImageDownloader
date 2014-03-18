using System;
namespace ImageDownloader.Core.Messages
{
    public class ShellMessage
    {
        public enum MessageTypes { SaveLayout, Exit, ShowTool, NewJob, CloseContent, CloseCurrent };

        public MessageTypes MessageType  { get; set; }
        public Type ToolType { get; set; }
        public IContent Content { get; set; }

        public ShellMessage(MessageTypes message_type)
        {
            MessageType = message_type;
        }

        public static ShellMessage ShowTool(Type tool_type)
        {
            return new ShellMessage(MessageTypes.ShowTool) { ToolType = tool_type };
        }

        public static ShellMessage CloseContent(IContent content)
        {
            return new ShellMessage(MessageTypes.CloseContent) { Content = content };
        }

        public static ShellMessage SaveLayout
        {
            get { return new ShellMessage(MessageTypes.SaveLayout); }
        }

        public static ShellMessage CloseCurrent
        {
            get { return new ShellMessage(MessageTypes.CloseCurrent); }
        }

        public static ShellMessage Exit
        {
            get { return new ShellMessage(MessageTypes.Exit); }
        }

        public static ShellMessage NewJob
        {
            get { return new ShellMessage(MessageTypes.NewJob); }
        }
    }
}
