using Lexfy.Domain;
using Lexfy.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository
{
    public class TreeRepository : Context<Tree>, ITreeRepository
    {
        public override Tree Adaptar(DataRow dataRow)
        {
            return new Tree
            {
                TreeId = Guid.Parse(dataRow["TreeId"].ToString()),
                Title = dataRow["Title"].ToString(),
                Description = dataRow["Description"] != DBNull.Value ? dataRow["Description"].ToString() : null,
                TreeChildId = dataRow["TreeChildId"] != DBNull.Value ? Guid.Parse(dataRow["TreeChildId"].ToString()) : Guid.Empty
            };
        }

        public override List<Tree> Adaptar(DataTable dataTable)
        {
            var trees = new List<Tree>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                trees.Add(Adaptar(dataTable.Rows[i]));

            return trees;
        }

        public Tree Get(Guid treeId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@TreeId", treeId) };

            var dataTable = ExecDataTable($@"SELECT TRE.TreeId,
                                                    TRE.Title,
                                                    TRE.Description,
                                                    TRE.TreeChildId
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tree] AS TRE
                                              WHERE 1=1
                                                    AND TRE.TreeId = @TreeId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Tree> Find(Tree tree)
        {
            return Adaptar(this.ExecDataTable($@"SELECT TRE.TreeId,
                                                        TRE.Title,
                                                        TRE.Description,
                                                        TRE.TreeChildId
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tree] AS TRE
                                                  WHERE 1=1 
                                                        AND (TRE.TreeId = @TreeId
                                                             OR @TreeId IS NULL)
                                                        AND (TRE.Title = @Title
                                                             OR @Title IS NULL)
                                                        AND (TRE.Description = @Description
                                                             OR @Description IS NULL)
                                                        AND (TRE.TreeChildId = @TreeChildId
                                                             OR @TreeChildId IS NULL)", CommandType.Text, Parameter(tree)));
        }

        public void Add(Tree tree)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tree] 
                                          (TreeId,
                                           Title,
                                           Description,
                                           TreeChildId)
                                   VALUES (@TreeId,
                                           @Title,
                                           @Description,
                                           @TreeChildId)", CommandType.Text, Parameter(tree));
        }

        /// <summary>
        /// Sempre será adicionado um novo registro, mesmo não edição, ou seja, se requisitado editar o registro é marcado como atualizado (U) e um novo registro é inserido
        /// </summary>
        /// <param name="tree"></param>
        public void Update(Tree tree)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tree]
                                  SET Title = COALESCE(@Title, Title),
                                      Description = COALESCE(@Description, Description),
                                      TreeChildId = COALESCE(@TreeChildId, TreeChildId),
                                WHERE 1=1
                                      AND TreeId = @TreeId", CommandType.Text, Parameter(tree));
        }

        public void Delete(Tree tree)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Tree] 
                                WHERE TreeId = @TreeId", CommandType.Text, Parameter(tree));
        }

        public void SoftDelete(Tree tree)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Tree tree)
        {
            if (tree != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@TreeId", tree.TreeId != Guid.Empty ? (object)tree.TreeId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@Title", !string.IsNullOrEmpty(tree.Title) ? (object)tree.Title : DBNull.Value),
                    new SqlParameter("@Description", !string.IsNullOrEmpty(tree.Description) ? (object)tree.Description : DBNull.Value),
                    new SqlParameter("@TreeChildId", tree.TreeChildId != Guid.Empty ? (object)tree.TreeChildId.ToString().ToLower() : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@TreeId", DBNull.Value),
                new SqlParameter("@Title", DBNull.Value),
                new SqlParameter("@Description", DBNull.Value),
                new SqlParameter("@TreeChildId", DBNull.Value)
            };
        }
    }
}