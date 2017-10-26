using Lexfy.Domain.Identity;
using System;

namespace Lexfy.Domain.Communication
{
    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid NotificationsGroupId { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime NotificationDate { get; set; }

        public Guid MessageId { get; set; }
        public virtual Message Message { get; set; }

        public DateTime? ReadingDate { get; set; }

        public Notification()
        {
            NotificationId = Guid.NewGuid();

            User = new User();
            UserId = Guid.Empty;

            NotificationDate = DateTime.Now;

            Message = new Message();
            MessageId = Guid.NewGuid();

            ReadingDate = null;
        }
    }
}