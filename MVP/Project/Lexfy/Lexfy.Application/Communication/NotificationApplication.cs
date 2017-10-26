using Lexfy.Application.Communication.Interfaces;
using Lexfy.Domain.Communication;
using Lexfy.Repository.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexfy.Application.Communication
{
    public class NotificationApplication : INotificationApplication
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMessageApplication _messageApplication;

        public NotificationApplication(INotificationRepository notificationRepository,
                                       IMessageApplication messageApplication)
        {
            _notificationRepository = notificationRepository;
            _messageApplication = messageApplication;
        }

        public Notification Get(Guid notificationId)
        {
            try
            {
                var notification = _notificationRepository.Get(notificationId);

                if (notification != null)
                    notification.Message = _messageApplication.Get(notification.MessageId);

                return notification;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Notification> Find(Notification notification)
        {
            try
            {
                return _notificationRepository.Find(notification).Select(item => new Notification()
                {
                    NotificationId = item.NotificationId,
                    NotificationsGroupId = item.NotificationsGroupId,
                    UserId = item.UserId,
                    NotificationDate = item.NotificationDate,
                    Message = _messageApplication.Get(item.MessageId),
                    ReadingDate = item.ReadingDate
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Guid Save(Notification notification)
        {
            var notificationId = Guid.Empty;

            // Notification já existe
            // Notification já existe, mesmo???
            if (notification.NotificationId != Guid.Empty
                && Get(notification.NotificationId) != null)
            {
                // Marca como atualizado (U) o Notification já existente
                // Sempre existirá apenas um registro com status A de cada PressAdvisoryService
                _notificationRepository.Update(new Notification()
                {
                    NotificationId = notification.NotificationId
                });

                // Adiciona novo Notification
                _notificationRepository.Add(new Notification()
                {
                    NotificationId = notification.NotificationId,
                    NotificationsGroupId = notification.NotificationsGroupId,
                    UserId = notification.UserId,
                    MessageId = _messageApplication.Save(notification.Message)
                });

                notificationId = notification.NotificationId;
            }
            // Notification não existe
            else
            {
                notificationId = Guid.NewGuid();

                // Adiciona novo Notification
                _notificationRepository.Add(new Notification()
                {
                    NotificationId = notification.NotificationId,
                    NotificationsGroupId = notification.NotificationsGroupId,
                    UserId = notification.UserId,
                    MessageId = _messageApplication.Save(notification.Message),
                    ReadingDate = notification.ReadingDate
                });
            }

            return notificationId;
        }

        public void SoftDelete(Notification notification)
        {
            try
            {
                _notificationRepository.SoftDelete(notification);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
