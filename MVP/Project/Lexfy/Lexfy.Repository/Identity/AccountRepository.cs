using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Identity
{
    public class AccountRepository : Context<Account>, IAccountRepository
    {
        public override Account Adaptar(DataRow dataRow)
        {
            return new Account
            {
                AccountId = Guid.Parse(dataRow["AccountId"].ToString()),
                CompanyId = Guid.Parse(dataRow["CompanyId"].ToString())
            };
        }

        public override List<Account> Adaptar(DataTable dataTable)
        {
            var accounts = new List<Account>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                accounts.Add(Adaptar(dataTable.Rows[i]));

            return accounts;
        }

        public Account Get(Guid accountId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@AccountId", accountId) };

            var dataTable = ExecDataTable($@"SELECT USR.AccountId,
                                                    USR.CompanyId
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Account] AS USR
                                              WHERE 1=1
                                                    AND USR.AccountId = @AccountId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Account> Find(Account account)
        {
            return Adaptar(this.ExecDataTable($@"SELECT USR.AccountId,
                                                        USR.CompanyId
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Account] AS USR
                                                  WHERE 1=1 
                                                        AND (USR.AccountId = @AccountId
                                                             OR @AccountId IS NULL)
                                                        AND (USR.CompanyId = @CompanyId
                                                             OR @CompanyId IS NULL)", CommandType.Text, Parameter(account)));
        }

        public void Add(Account account)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Account] 
                                          (AccountId,
                                           CompanyId)
                                   VALUES (@AccountId,
                                           @Name)", CommandType.Text, Parameter(account));
        }

        public void Update(Account account)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Account]
                                  SET CompanyId = COALESCE(@CompanyId, CompanyId)
                                WHERE 1=1
                                      AND AccountId = @AccountId", CommandType.Text, Parameter(account));
        }

        public void Delete(Account account)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Account] 
                                WHERE AccountId = @AccountId", CommandType.Text, Parameter(account));
        }

        public void SoftDelete(Account account)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Account account)
        {
            if (account != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@AccountId", account.AccountId != Guid.Empty ? (object)account.AccountId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@CompanyId", account.CompanyId != Guid.Empty ? (object)account.CompanyId.ToString().ToLower() : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@AccountId", DBNull.Value),
                new SqlParameter("@CompanyId", DBNull.Value)
            };
        }
    }
}
