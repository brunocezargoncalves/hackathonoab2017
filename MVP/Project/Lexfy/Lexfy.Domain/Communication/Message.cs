using System;

namespace Lexfy.Domain.Communication
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string Subject { get; set; }
        public string MessageClean { get; set; }
        public string MessageHtml { get; set; }

        public Message()
        {
            MessageId = Guid.NewGuid();
        }
    }
}