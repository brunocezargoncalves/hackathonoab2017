using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Identity
{
    public class UserTypeRepository : Context<UserType>, IUserTypeRepository
    {
        public override UserType Adaptar(DataRow dataRow)
        {
            return new UserType
            {
                UserTypeId = Guid.Parse(dataRow["UserTypeId"].ToString()),
                Name = dataRow["Name"] != DBNull.Value ? dataRow["Name"].ToString() : null
            };
        }

        public override List<UserType> Adaptar(DataTable dataTable)
        {
            var userTypes = new List<UserType>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                userTypes.Add(Adaptar(dataTable.Rows[i]));

            return userTypes;
        }

        public UserType Get(Guid userTypeId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@UserTypeId", userTypeId) };

            var dataTable = ExecDataTable($@"SELECT USR.UserTypeId,
                                                    USR.Name
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_UserType] AS USR
                                              WHERE 1=1
                                                    AND USR.UserTypeId = @UserTypeId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<UserType> Find(UserType userType)
        {
            return Adaptar(this.ExecDataTable($@"SELECT USR.UserTypeId,
                                                        USR.Name
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_UserType] AS USR
                                                  WHERE 1=1 
                                                        AND (USR.UserTypeId = @UserTypeId
                                                             OR @UserTypeId IS NULL)
                                                        AND (USR.Name = @Name
                                                             OR @Name IS NULL)", CommandType.Text, Parameter(userType)));
        }

        public void Add(UserType userType)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_UserType] 
                                          (UserTypeId,
                                           Name)
                                   VALUES (@UserTypeId,
                                           @Name)", CommandType.Text, Parameter(userType));
        }

        public void Update(UserType userType)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_UserType]
                                  SET Name = COALESCE(@Name, Name)
                                WHERE 1=1
                                      AND UserTypeId = @UserTypeId", CommandType.Text, Parameter(userType));
        }

        public void Delete(UserType userType)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_UserType] 
                                WHERE UserTypeId = @UserTypeId", CommandType.Text, Parameter(userType));
        }

        public void SoftDelete(UserType userType)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(UserType userType)
        {
            if (userType != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@UserTypeId", userType.UserTypeId != Guid.Empty ? (object)userType.UserTypeId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@Name", !string.IsNullOrEmpty(userType.Name) ? (object)userType.Name : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@UserTypeId", DBNull.Value),
                new SqlParameter("@Name", DBNull.Value)
            };
        }
    }
}
