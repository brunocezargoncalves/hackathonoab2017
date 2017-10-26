using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexfy.Repository
{
    public abstract class Context<T> : IDisposable
    {
        private SqlConnection Connection { get; set; }
        private SqlCommand Command { get; set; }
        private SqlDataReader Reader { get; set; }

        protected Context()
        {
            Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LexfyConnection_" + ConfigurationManager.AppSettings["environment"]].ConnectionString);
        }

        private void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
        }

        public void ExecuteNonQuery(string command, CommandType commandType)
        {
            OpenConnection();
            Command = new SqlCommand(command, Connection) { CommandType = commandType };
            Command.ExecuteNonQuery();
        }

        public void ExecuteNonQuery(string command, CommandType commandType, List<SqlParameter> parametros)
        {
            OpenConnection();
            Command = new SqlCommand(command, Connection);

            foreach (var item in parametros)
                Command.Parameters.Add(item);

            Command.CommandType = commandType;
            Command.ExecuteNonQuery();
        }

        public int ExecuteScalar(string command, CommandType commandType)
        {
            OpenConnection();
            Command = new SqlCommand(command, Connection) { CommandType = commandType };
            return (int)Command.ExecuteScalar();
        }

        public DataTable ExecDataTable(string command, CommandType commandType)
        {
            OpenConnection();
            Command = new SqlCommand(command, Connection) { CommandType = commandType };
            var adap = new SqlDataAdapter { SelectCommand = Command };
            var dt = new DataTable();
            adap.Fill(dt);
            return dt;
        }

        public DataTable ExecDataTable(string command, CommandType commandType, List<SqlParameter> parametros)
        {
            OpenConnection();
            Command = new SqlCommand(command, Connection);

            foreach (var item in parametros)
                Command.Parameters.Add(item);

            Command.CommandType = commandType;
            var adap = new SqlDataAdapter { SelectCommand = Command };
            var dt = new DataTable();
            adap.Fill(dt);
            return dt;
        }

        public abstract List<T> Adaptar(DataTable dataTable);
        public abstract T Adaptar(DataRow dataRow);

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}