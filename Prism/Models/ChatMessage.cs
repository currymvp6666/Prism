using System.Windows;

namespace Prism.Model
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        // 水平对齐，用户消息靠右，AI消息靠左
        public HorizontalAlignment Alignment => Sender == "你" ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }
}
