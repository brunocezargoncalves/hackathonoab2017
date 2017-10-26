using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;
using System.Configuration;

namespace Lexfy.Repository
{
    public class TagRepository : Context<Tag>, ITagRepository
    {
        public override Tag Adaptar(DataRow dataRow)
        {
            return new Tag
            {
                TagId = Guid.Parse(dataRow["TagId"].ToString()),
                Name = dataRow["Name"].ToString()
            };
        }

        public override List<Tag> Adaptar(DataTable dataTable)
        {
            var tags = new List<Tag>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                tags.Add(Adaptar(dataTable.Rows[i]));

            return tags;
        }

        public Tag Get(Guid tagId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@TagId", tagId) };

            var dataTable = ExecDataTable($@"SELECT BRC.TagId,
                                                    BRC.Name
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tag] AS BRC
                                              WHERE 1=1
                                                    AND BRC.TagId = @TagId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Tag> Find(Tag tag)
        {
            return Adaptar(this.ExecDataTable($@"SELECT BRC.TagId,
                                                        BRC.Name
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tag] AS BRC
                                                  WHERE 1=1 
                                                        AND (BRC.TagId = @TagId
                                                             OR @TagId IS NULL)
                                                        AND (BRC.Name = @Name
                                                             OR @Name IS NULL)", CommandType.Text, Parameter(tag)));
        }

        public void Add(Tag tag)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tag] 
                                          (TagId,
                                           Name)
                                   VALUES (@TagId,
                                           @Name)", CommandType.Text, Parameter(tag));
        }

        public void Update(Tag tag)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tag]
                                  SET Name = COALESCE(@Name, Name)
                                WHERE 1=1
                                      AND TagId = @TagId", CommandType.Text, Parameter(tag));
        }

        public void Delete(Tag tag)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tag] 
                                WHERE TagId = @TagId", CommandType.Text, Parameter(tag));
        }

        public void SoftDelete(Tag tag)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Tag tag)
        {
            if (tag != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@TagId", tag.TagId != Guid.Empty ? (object)tag.TagId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@Name", !string.IsNullOrEmpty(tag.Name) ? (object)tag.Name : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@TagId", DBNull.Value),
                new SqlParameter("@Name", DBNull.Value)
            };
        }
    }
}
