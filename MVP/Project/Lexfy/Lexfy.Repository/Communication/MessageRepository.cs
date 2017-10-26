using Lexfy.Domain.Communication;
using Lexfy.Repository.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Communication
{
    public class MessageRepository : Context<Message>, IMessageRepository
    {
        public override Message Adaptar(DataRow dataRow)
        {
            return new Message()
            {
                MessageId = Guid.Parse(dataRow["MessageId"].ToString()),
                Subject = dataRow["Subject"].ToString(),
                MessageClean = dataRow["MessageClean"].ToString(),
                MessageHtml = dataRow["MessageHtml"] != DBNull.Value ? dataRow["MessageHtml"].ToString() : null
            };
        }

        public override List<Message> Adaptar(DataTable dataTable)
        {
            var messages = new List<Message>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                messages.Add(Adaptar(dataTable.Rows[i]));

            return messages;
        }

        public Message Get(Guid messageId)
        {
            var parameter = new List<SqlParameter>() { new SqlParameter("@MessageId", messageId) };

            var dataTable = ExecDataTable($@"SELECT MSG.MessageId,
                                                    MSG.Subject,
	                                                MSG.MessageClean,
                                                    MSG.MessageHtml
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Message] AS MSG 
                                              WHERE 1=1
                                                    AND MSG.MessageId = @MessageId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Message> Find(Message message)
        {
            return Adaptar(this.ExecDataTable($@"SELECT MSG.MessageId,
                                                        MSG.Subject,
	                                                    MSG.MessageClean,
                                                        MSG.MessageHtml
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Message] AS MSG 
                                                  WHERE 1=1
                                                        AND (MSG.MessageId = @MessageId
                                                             OR @MessageId IS NULL)
                                                        AND (MSG.Subject = @Subject
                                                             OR @Subject IS NULL)
                                                        AND (MSG.MessageClean = @MessageClean
                                                             OR @MessageClean IS NULL)
                                                        AND (MSG.MessageHtml = @MessageHtml
                                                             OR @MessageHtml IS NULL)", CommandType.Text, Parameter(message)));
        }

        public void Add(Message message)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Message]
                                          (MessageId,
                                           Subject,
	                                       MessageClean,
                                           MessageHtml)
                                   VALUES (@MessageId,
                                           @Subject,
                                           @MessageClean,
                                           @MessageHtml)", CommandType.Text, Parameter(message));
        }

        public void Update(Message message)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Message]
                                  SET ModifyBy = COALESCE(@ModifyBy, ModifyBy),
                                      ModifyDate = COALESCE(@ModifyDate, ModifyDate),
                                      Status = 'U'
                                WHERE 1=1
                                      AND Status = 'A'
                                      AND MessageId = @MessageId", CommandType.Text, Parameter(message));
        }

        public void Delete(Message message)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Message] 
                                WHERE MessageId = @MessageId", CommandType.Text, Parameter(message));
        }

        public void SoftDelete(Message message)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Message] 
                                  SET ModifyBy = COALESCE(@ModifyBy, ModifyBy),
                                      ModifyDate = COALESCE(@ModifyDate, ModifyDate),
                                      Status = 'D'
                                WHERE 1=1
                                      AND Status = 'A'
                                      AND MessageId = @MessageId", CommandType.Text, Parameter(message));
        }

        public List<SqlParameter> Parameter(Message message)
        {
            if (message != null)
            {
                return new List<SqlParameter>(){
                    new SqlParameter("@MessageId", message.MessageId != Guid.Empty ? (object)message.MessageId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@Subject", !string.IsNullOrEmpty(message.Subject)  ? (object)message.Subject : DBNull.Value),
                    new SqlParameter("@MessageClean", !string.IsNullOrEmpty(message.MessageClean)  ? (object)message.MessageClean : DBNull.Value),
                    new SqlParameter("@MessageHtml", !string.IsNullOrEmpty(message.MessageHtml)  ? (object)message.MessageHtml : DBNull.Value)
                };
            }

            return new List<SqlParameter>(){
                new SqlParameter("@MessageId", DBNull.Value),
                new SqlParameter("@Subject", DBNull.Value),
                new SqlParameter("@MessageClean", DBNull.Value),
                new SqlParameter("@MessageHtml", DBNull.Value)
            };
        }
    }
}