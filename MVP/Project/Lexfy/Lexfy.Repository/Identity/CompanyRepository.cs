using Lexfy.Domain.Identity;
using Lexfy.Repository.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository.Identity
{
    public class CompanyRepository : Context<Company>, ICompanyRepository
    {
        public override Company Adaptar(DataRow dataRow)
        {
            return new Company
            {
                CompanyId = Guid.Parse(dataRow["CompanyId"].ToString())
            };
        }

        public override List<Company> Adaptar(DataTable dataTable)
        {
            var companys = new List<Company>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                companys.Add(Adaptar(dataTable.Rows[i]));

            return companys;
        }

        public Company Get(Guid companyId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@CompanyId", companyId) };

            var dataTable = ExecDataTable($@"SELECT USR.CompanyId
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Company] AS USR
                                              WHERE 1=1
                                                    AND USR.CompanyId = @CompanyId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Company> Find(Company company)
        {
            return Adaptar(this.ExecDataTable($@"SELECT USR.CompanyId
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Company] AS USR
                                                  WHERE 1=1 
                                                        AND (USR.CompanyId = @CompanyId
                                                             OR @CompanyId IS NULL)", CommandType.Text, Parameter(company)));
        }

        public void Add(Company company)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Company] 
                                          (CompanyId)
                                   VALUES (@CompanyId)", CommandType.Text, Parameter(company));
        }

        public void Update(Company company)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Company]
                                  SET CompanyName = COALESCE(@CompanyName, CompanyName)                                      
                                WHERE 1=1
                                      AND CompanyId = @CompanyId", CommandType.Text, Parameter(company));
        }

        public void Delete(Company company)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Company] 
                                WHERE CompanyId = @CompanyId", CommandType.Text, Parameter(company));
        }

        public void SoftDelete(Company company)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Company company)
        {
            if (company != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@CompanyId", company.CompanyId != Guid.Empty ? (object)company.CompanyId.ToString().ToLower() : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@CompanyId", DBNull.Value)
            };
        }
    }
}
