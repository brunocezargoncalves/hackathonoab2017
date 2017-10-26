using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lexfy.Domain;
using Lexfy.Repository.Interfaces;
using System.Configuration;

namespace Lexfy.Repository
{
    public class NodeRepository : Context<Node>, INodeRepository
    {
        public override Node Adaptar(DataRow dataRow)
        {
            return new Node
            {
                NodeId = Guid.Parse(dataRow["NodeId"].ToString()),
                BranchId = Guid.Parse(dataRow["BranchId"].ToString())
            };
        }

        public override List<Node> Adaptar(DataTable dataTable)
        {
            var nodes = new List<Node>();

            for (var i = 0; i < dataTable.Rows.Count; i++)
                nodes.Add(Adaptar(dataTable.Rows[i]));

            return nodes;
        }

        public Node Get(Guid nodeId)
        {
            var parameter = new List<SqlParameter> { new SqlParameter("@NodeId", nodeId) };

            var dataTable = ExecDataTable($@"SELECT NDE.NodeId,
                                                    NDE.BranchId
                                               FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Node] AS NDE
                                              WHERE 1=1
                                                    AND NDE.NodeId = @NodeId", CommandType.Text, parameter);

            return dataTable.Rows.Count > 0 ? Adaptar(dataTable.Rows[0]) : null;
        }

        public List<Node> Find(Node node)
        {
            return Adaptar(this.ExecDataTable($@"SELECT NDE.NodeId,
                                                        NDE.BranchId
                                                   FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Node] AS NDE
                                                  WHERE 1=1 
                                                        AND (NDE.NodeId = @NodeId
                                                             OR @NodeId IS NULL)
                                                        AND (NDE.BranchId = @BranchId
                                                             OR @BranchId IS NULL)", CommandType.Text, Parameter(node)));
        }

        public void Add(Node node)
        {
            ExecuteNonQuery($@"INSERT INTO [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Node] 
                                          (NodeId,
                                           BranchId)
                                   VALUES (@NodeId,
                                           @BranchId)", CommandType.Text, Parameter(node));
        }

        public void Update(Node node)
        {
            ExecuteNonQuery($@"UPDATE [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Node]
                                  SET BranchId = COALESCE(@BranchId, BranchId)
                                WHERE 1=1
                                      AND NodeId = @NodeId", CommandType.Text, Parameter(node));
        }

        public void Delete(Node node)
        {
            ExecuteNonQuery($@"DELETE 
                                 FROM [dbo].[Lexfy_{ ConfigurationManager.AppSettings["environment"] }_Node] 
                                WHERE NodeId = @NodeId", CommandType.Text, Parameter(node));
        }

        public void SoftDelete(Node node)
        {
            throw new NotImplementedException();
        }

        public List<SqlParameter> Parameter(Node node)
        {
            if (node != null)
            {
                return new List<SqlParameter>
                {
                    new SqlParameter("@NodeId", node.NodeId != Guid.Empty ? (object)node.NodeId.ToString().ToLower() : DBNull.Value),
                    new SqlParameter("@BranchId", node.BranchId != Guid.Empty ? (object)node.BranchId.ToString().ToLower() : DBNull.Value)
                };
            }

            return new List<SqlParameter>
            {
                new SqlParameter("@NodeId", DBNull.Value),
                new SqlParameter("@BranchId", DBNull.Value)
            };
        }
    }
}
