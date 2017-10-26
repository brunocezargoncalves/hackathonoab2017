using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;
using System.Configuration;

namespace Lexfy.Repository
{
    public class BranchRepository : Context<Branch>, IBranchRepository
    {
        public override Branch Adaptar(DataRow dataRow)
        {
            return new Branch
            {
                BranchId = Guid.Parse(dataRow["BranchId"].ToString()),
                TreeId = Guid.Parse(dataRow["TreeId"].ToString()),
                Index = Convert.ToInt32(dataRow["Index"]),
                Title = dataRow["Title"].ToString(),
                Description = dataRow["Description"] != DBNull.Value ? dataRow["Description"].ToString() : null,
                BranchChildId = dataRow["BranchChildId"] != DBNull.Value ? Guid.Parse(dataRow["BranchChildId"].ToString()) : Guid.Empty
            };
        }

        public override List<Branch> Adaptar(DataTable dataTable)
        {
            var branches = new List<Branch>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                branches.Add(Adaptar(dataTable.Rows[i]));

            return branches;
        }

        public Branch Get(Guid branchId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@BranchId", branchId) };

            var dataTable = ExecDataTable($@"SELECT BRC.BranchId,
                                                    BRC.TreeId,
                                                    BRC.Index,
                                                    BRC.Title,
                                                    BRC.Description,
                                                    BRC.BranchChildId
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Branch] AS BRC
                                              WHERE 1=1
                                                    AND BRC.BranchId = @BranchId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Branch> Find(Branch branch)
        {
            return Adaptar(this.ExecDataTable($@"SELECT BRC.BranchId,
                                                        BRC.TreeId,
                                                        BRC.Index,
                                                        BRC.Title,
                                                        BRC.Description,
                                                        BRC.BranchChildId
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Branch] AS BRC
                                                  WHERE 1=1 
                                                        AND (BRC.BranchId = @BranchId
                                                             OR @BranchId IS NULL)
                                                        AND (BRC.TreeId = @TreeId
                                                             OR @TreeId IS NULL)
                                                        AND (BRC.Index = @Index
                                                             OR @Index IS NULL)
                                                        AND (BRC.Title = @Title
                                                             OR @Title IS NULL)
                                                        AND (BRC.Description = @Description
                                                             OR @Description IS NULL)
                                                        AND (BRC.BranchChildId = @BranchChildId
                                                             OR @BranchChildId IS NULL)", CommandType.Text, Parameter(branch)));
        }

        public void Add(Branch branch)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Branch] 
                                          (BranchId,
                                           TreeId,
                                           Index,
                                           Title,
                                           Description,
                                           BranchChildId)
                                   VALUES (@BranchId,
                                           @TreeId,
                                           @Index,
                                           @Title,
                                           @Description,
                                           @BranchChildId)", CommandType.Text, Parameter(branch));
        }

        public void Update(Branch branch)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Branch]
                                  SET TreeId = COALESCE(@TreeId, TreeId),
                                      Index = COALESCE(@Index, Index),
                                      Title = COALESCE(@Title, Title),
                                      Description = COALESCE(@Description, Description),
                                      BranchChildId = COALESCE(@BranchChildId, BranchChildId)
                                WHERE 1=1
                                      AND BranchId = @BranchId", CommandType.Text, Parameter(branch));
        }

        public void Delete(Branch branch)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Branch] 
                                WHERE BranchId = @BranchId", CommandType.Text, Parameter(branch));
        }

        public void SoftDelete(Branch branch)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Branch branch)
        {
            if (branch != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@BranchId", branch.BranchId != Guid.Empty ? (object)branch.BranchId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@TreeId", branch.TreeId != Guid.Empty ? (object)branch.TreeId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@Index", branch.Index != int.MinValue ? (object)branch.Index : DBNull.Value),
                    new SqlParameter("@Title", !string.IsNullOrEmpty(branch.Title) ? (object)branch.Title : DBNull.Value),
                    new SqlParameter("@Description", !string.IsNullOrEmpty(branch.Description) ? (object)branch.Description : DBNull.Value),
                    new SqlParameter("@BranchChildId", branch.BranchChildId != Guid.Empty ? (object)branch.BranchChildId.ToString().ToLower() : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@BranchId", DBNull.Value),
                new SqlParameter("@TreeId", DBNull.Value),
                new SqlParameter("@Index", DBNull.Value),
                new SqlParameter("@Title", DBNull.Value),
                new SqlParameter("@Description", DBNull.Value),
                new SqlParameter("@BranchChildId", DBNull.Value)
            };
        }
    }
}
