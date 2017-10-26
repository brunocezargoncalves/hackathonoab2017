using Lexfy.Domain.Communication;
using Lexfy.Repository.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Communication
{
    public class NotificationRepository : Context<Notification>, INotificationRepository
    {
        public override Notification Adaptar(DataRow dataRow)
        {
            return new Notification()
            {
                NotificationId = Guid.Parse(dataRow["NotificationId"].ToString()),
                NotificationsGroupId = Guid.Parse(dataRow["NotificationsGroupId"].ToString()),
                UserId = Guid.Parse(dataRow["UserId"].ToString()),
                NotificationDate = (DateTime)dataRow["NotificationDate"],
                MessageId = Guid.Parse(dataRow["MessageId"].ToString()),
                ReadingDate = dataRow["ReadingDate"] != DBNull.Value ? (DateTime?)dataRow["ReadingDate"] : null
            };
        }

        public override List<Notification> Adaptar(DataTable dataTable)
        {
            var notifications = new List<Notification>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                notifications.Add(Adaptar(dataTable.Rows[i]));

            return notifications;
        }

        public Notification Get(Guid notificationId)
        {
            var parameter = new List<SqlParameter>() { new SqlParameter("@NotificationId", notificationId) };

            var dataTable = ExecDataTable($@"SELECT NTF.NotificationId,
                                                    NTF.NotificationsGroupId,
	                                                NTF.UserId,
                                                    NTF.NotificationDate,
                                                    NTF.MessageId,
                                                    NTF.ReadingDate
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Notification] AS NTF 
                                              WHERE 1=1
                                                    AND NTF.NotificationId = @NotificationId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Notification> Find(Notification notification)
        {
            return Adaptar(this.ExecDataTable($@"SELECT NTF.NotificationId,
                                                        NTF.NotificationsGroupId,
	                                                    NTF.UserId,
                                                        NTF.NotificationDate,
                                                        NTF.MessageId,
                                                        NTF.ReadingDate
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Notification] AS NTF 
                                                  WHERE 1=1
                                                        AND (NTF.UserId = @UserId
                                                             OR @UserId IS NULL)
                                                        AND (NTF.ReadingDate = @ReadingDate
                                                             OR @ReadingDate IS NULL)
                                               ORDER BY NotificationDate DESC", CommandType.Text, Parameter(notification)));
        }

        public void Add(Notification notification)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Notification]
                                          (NotificationId,
                                           NotificationsGroupId,
	                                       UserId,
                                           NotificationDate,
                                           MessageId,
                                           ReadingDate)
                                   VALUES (@NotificationId,
                                           @NotificationsGroupId,
                                           @UserId,
                                           @NotificationDate,
                                           @MessageId,
                                           @ReadingDate)", CommandType.Text, Parameter(notification));
        }

        public void Update(Notification notification)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Notification]
                                  SET ModifyBy = COALESCE(@ModifyBy, ModifyBy),
                                      ModifyDate = COALESCE(@ModifyDate, ModifyDate),
                                      Status = 'U'
                                WHERE 1=1
                                      AND Status = 'A'
                                      AND NotificationID = @NotificationID", CommandType.Text, Parameter(notification));
        }

        public void Delete(Notification notification)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Notification] 
                                WHERE NotificationId = @NotificationId", CommandType.Text, Parameter(notification));
        }

        public void SoftDelete(Notification notification)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Notification] 
                                  SET ModifyBy = COALESCE(@ModifyBy, ModifyBy),
                                      ModifyDate = COALESCE(@ModifyDate, ModifyDate),
                                      Status = 'D'
                                WHERE 1=1
                                      AND Status = 'A'
                                      AND NotificationID = @NotificationID", CommandType.Text, Parameter(notification));
        }

        public List<SqlParameter> Parameter(Notification notification)
        {
            if (notification != null)
            {
                return new List<SqlParameter>(){
                    new SqlParameter("@NotificationId", notification.NotificationId != Guid.Empty ? (object)notification.NotificationId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@NotificationsGroupId", notification.NotificationsGroupId != Guid.Empty ? (object)notification.NotificationsGroupId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@UserId", notification.UserId != Guid.Empty ? (object)notification.UserId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@NotificationDate", notification.NotificationDate != DateTime.MinValue ? (object)notification.NotificationDate : DBNull.Value),
                    new SqlParameter("@MessageId", notification.MessageId != Guid.Empty ? (object)notification.MessageId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@ReadingDate", notification.ReadingDate != null && notification.ReadingDate != DateTime.MinValue ? (object)notification.ReadingDate : DBNull.Value)
                };
            }

            return new List<SqlParameter>(){
                new SqlParameter("@NotificationId", DBNull.Value),
                new SqlParameter("@NotificationsGroupId", DBNull.Value),
                new SqlParameter("@UserId", DBNull.Value),
                new SqlParameter("@NotificationDate", DBNull.Value),
                new SqlParameter("@MessageId", DBNull.Value),
                new SqlParameter("@ReadingDate", DBNull.Value)
            };
        }
    }
}