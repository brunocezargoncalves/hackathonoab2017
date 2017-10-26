using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Identity
{
    public class AccountTypeRepository : Context<AccountType>, IAccountTypeRepository
    {
        public override AccountType Adaptar(DataRow dataRow)
        {
            return new AccountType
            {
                AccountTypeId = Guid.Parse(dataRow["AccountTypeId"].ToString()),
                Name = dataRow["Name"] != DBNull.Value ? dataRow["Name"].ToString() : null
            };
        }

        public override List<AccountType> Adaptar(DataTable dataTable)
        {
            var accountTypes = new List<AccountType>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                accountTypes.Add(Adaptar(dataTable.Rows[i]));

            return accountTypes;
        }

        public AccountType Get(Guid accountTypeId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@AccountTypeId", accountTypeId) };

            var dataTable = ExecDataTable($@"SELECT USR.AccountTypeId,
                                                    USR.Name
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_AccountType] AS USR
                                              WHERE 1=1
                                                    AND USR.AccountTypeId = @AccountTypeId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<AccountType> Find(AccountType accountType)
        {
            return Adaptar(this.ExecDataTable($@"SELECT USR.AccountTypeId,
                                                        USR.Name
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_AccountType] AS USR
                                                  WHERE 1=1 
                                                        AND (USR.AccountTypeId = @AccountTypeId
                                                             OR @AccountTypeId IS NULL)
                                                        AND (USR.Name = @Name
                                                             OR @Name IS NULL)", CommandType.Text, Parameter(accountType)));
        }

        public void Add(AccountType accountType)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_AccountType] 
                                          (AccountTypeId,
                                           Name)
                                   VALUES (@AccountTypeId,
                                           @Name)", CommandType.Text, Parameter(accountType));
        }

        public void Update(AccountType accountType)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_AccountType]
                                  SET Name = COALESCE(@Name, Name)
                                WHERE 1=1
                                      AND AccountTypeId = @AccountTypeId", CommandType.Text, Parameter(accountType));
        }

        public void Delete(AccountType accountType)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_AccountType] 
                                WHERE AccountTypeId = @AccountTypeId", CommandType.Text, Parameter(accountType));
        }

        public void SoftDelete(AccountType accountType)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(AccountType accountType)
        {
            if (accountType != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@AccountTypeId", accountType.AccountTypeId != Guid.Empty ? (object)accountType.AccountTypeId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@Name", !string.IsNullOrEmpty(accountType.Name) ? (object)accountType.Name : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@AccountTypeId", DBNull.Value),
                new SqlParameter("@Name", DBNull.Value)
            };
        }
    }
}
