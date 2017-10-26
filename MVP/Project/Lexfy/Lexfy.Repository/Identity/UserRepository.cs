using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Identity
{
    public class UserRepository : Context<User>, IUserRepository
    {
        public override User Adaptar(DataRow dataRow)
        {
            return new User
            {
                UserId = Guid.Parse(dataRow["UserId"].ToString()),
                UserName = dataRow["UserName"] != DBNull.Value ? dataRow["UserName"].ToString() : null,
                PasswordHash = dataRow["PasswordHash"] != DBNull.Value ? dataRow["PasswordHash"].ToString() : null,
                AccountId = Guid.Parse(dataRow["AccountId"].ToString()),
                ProfileId = Guid.Parse(dataRow["ProfileId"].ToString()),
                UserTypeId = Guid.Parse(dataRow["UserTypeId"].ToString()),
                IsConfirmed = Convert.ToBoolean(dataRow["IsConfirmed"]),
                IsActive = Convert.ToBoolean(dataRow["IsActive"]),
                ResetToken = dataRow["ResetToken"] != DBNull.Value ? (Guid?)Guid.Parse(dataRow["ResetToken"].ToString()) : null,
                ResetTokenExpiration = dataRow["ResetTokenExpiration"] != DBNull.Value ? (DateTime?)dataRow["ResetTokenExpiration"] : null
            };
        }

        public override List<User> Adaptar(DataTable dataTable)
        {
            var users = new List<User>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                users.Add(Adaptar(dataTable.Rows[i]));

            return users;
        }

        public User Get(Guid userId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@UserId", userId) };

            var dataTable = ExecDataTable($@"SELECT USR.UserId,
                                                    USR.UserName,
                                                    USR.PasswordHash,
                                                    USR.AccountId,
                                                    USR.ProfileId,
                                                    USR.UserTypeId,
                                                    USR.IsConfirmed,
                                                    USR.IsActive,
                                                    USR.ResetToken,
                                                    USR.ResetTokenExpiration
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_User] AS USR
                                              WHERE 1=1
                                                    AND USR.UserId = @UserId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<User> Find(User user)
        {
            return Adaptar(this.ExecDataTable($@"SELECT USR.UserId,
                                                        USR.UserName,
                                                        USR.PasswordHash,
                                                        USR.AccountId,
                                                        USR.ProfileId,
                                                        USR.UserTypeId,
                                                        USR.IsConfirmed,
                                                        USR.IsActive,
                                                        USR.ResetToken,
                                                        USR.ResetTokenExpiration
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_User] AS USR
                                                  WHERE 1=1 
                                                        AND (USR.UserId = @UserId
                                                             OR @UserId IS NULL)
                                                        AND (USR.UserName = @UserName
                                                             OR @UserName IS NULL)
                                                        AND (USR.PasswordHash = @PasswordHash
                                                             OR @PasswordHash IS NULL)
                                                        AND (USR.AccountId = @AccountId
                                                             OR @AccountId IS NULL)
                                                        AND (USR.ProfileId = @ProfileId
                                                             OR @ProfileId IS NULL)
                                                        AND (USR.UserTypeId = @UserTypeId
                                                             OR @UserTypeId IS NULL)
                                                        AND (USR.IsConfirmed = @IsConfirmed
                                                             OR @IsConfirmed IS NULL)
                                                        AND (USR.IsActive = @IsActive
                                                             OR @IsActive IS NULL)
                                                        AND (USR.ResetToken = @ResetToken
                                                             OR @ResetToken IS NULL)
                                                        AND (USR.ResetTokenExpiration = @ResetTokenExpiration
                                                             OR @ResetTokenExpiration IS NULL)", CommandType.Text, Parameter(user)));
        }

        public void Add(User user)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_User] 
                                          (UserId,
                                           UserName,
                                           PasswordHash,
                                           AccountId,
                                           ProfileId,
                                           UserTypeId,
                                           IsConfirmed,
                                           IsActive,
                                           ResetToken,
                                           ResetTokenExpiration)
                                   VALUES (@UserId,
                                           @UserName,
                                           @PasswordHash,
                                           @AccountId,
                                           @ProfileId,
                                           @UserTypeId,
                                           @IsConfirmed,
                                           @IsActive,
                                           @ResetToken,
                                           @ResetTokenExpiration)", CommandType.Text, Parameter(user));
        }

        public void Update(User user)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_User]
                                  SET UserName = COALESCE(@UserName, UserName),
                                      PasswordHash = COALESCE(@PasswordHash, PasswordHash),
                                      AccountId = COALESCE(@AccountId, AccountId),
                                      ProfileId = COALESCE(@ProfileId, ProfileId),
                                      UserTypeId = COALESCE(@UserTypeId, UserTypeId),
                                      IsConfirmed = COALESCE(@IsConfirmed, IsConfirmed),
                                      IsActive = COALESCE(@IsActive, IsActive),
                                      ResetToken = COALESCE(@ResetToken, ResetToken)
                                      ResetTokenExpiration = COALESCE(@ResetTokenExpiration, ResetTokenExpiration)
                                WHERE 1=1
                                      AND UserId = @UserId", CommandType.Text, Parameter(user));
        }

        public void Delete(User user)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_User] 
                                WHERE UserId = @UserId", CommandType.Text, Parameter(user));
        }

        public void SoftDelete(User user)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(User user)
        {
            if (user != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@UserId", user.UserId != Guid.Empty ? (object)user.UserId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@UserName", !string.IsNullOrEmpty(user.UserName) ? (object)user.UserName : DBNull.Value),
                    new SqlParameter("@PasswordHash", !string.IsNullOrEmpty(user.PasswordHash) ? (object)user.PasswordHash : DBNull.Value),
                    new SqlParameter("@AccountId", user.AccountId != Guid.Empty ? (object)user.AccountId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@ProfileId", user.ProfileId != Guid.Empty ? (object)user.ProfileId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@UserTypeId", user.UserTypeId != Guid.Empty ? (object)user.UserTypeId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@IsConfirmed", user.IsConfirmed),
                    new SqlParameter("@IsActive", user.IsActive),
                    new SqlParameter("@ResetToken", (user.ResetToken != null && user.ResetToken != Guid.Empty) ? (object)user.ResetToken.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@ResetTokenExpiration", (user.ResetTokenExpiration != null && user.ResetTokenExpiration != DateTime.MinValue) ? (object)user.ResetTokenExpiration : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@UserId", DBNull.Value),
                new SqlParameter("@UserName", DBNull.Value),
                new SqlParameter("@PasswordHash", DBNull.Value),
                new SqlParameter("@AccountId", DBNull.Value),
                new SqlParameter("@ProfileId", DBNull.Value),
                new SqlParameter("@UserTypeId", DBNull.Value),
                new SqlParameter("@IsConfirmed", DBNull.Value),
                new SqlParameter("@IsActive", DBNull.Value),
                new SqlParameter("@ResetToken", DBNull.Value),
                new SqlParameter("@ResetTokenExpiration", DBNull.Value)
            };
        }

        public User ForgotPassword(string userName)
        {
            var parameter = new List<SqlParameter>() { new SqlParameter("@UserName", userName) };

            var dataTable = ExecDataTable($@"SELECT USR.UserId,
	                                                USR.UserName,
	                                                USR.PasswordHash,
	                                                USR.ProfileID,
                                                    USR.Type,
                                                    USR.IsActive,
                                                    USR.ResetToken,
                                                    USR.ResetTokenExpiration
                                               FROM [dbo].[PressOffice_{ ConfigurationManager.AppSettings["environment"] }_User] AS USR 
                                              WHERE 1=1
                                                    AND USR.UserName = @UserName", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }
    }
}
