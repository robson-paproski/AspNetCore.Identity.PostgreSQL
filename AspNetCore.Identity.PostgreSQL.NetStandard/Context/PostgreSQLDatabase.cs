using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;

namespace AspNetCore.Identity.PostgreSQL.Context
{
    public class PostgreSQLDatabase
    {
        private static NpgsqlConnection _connection;
        private readonly IConfiguration _configurationRoot;
        private static object _consulta = new object();

        public PostgreSQLDatabase(IConfiguration configurationRoot)
        {
            _configurationRoot = configurationRoot;
            if (string.IsNullOrEmpty(IdentityDbConfig.StringConnectionName))
                IdentityDbConfig.StringConnectionName = "PostgreSQLBaseConnection";
            try
            {
                _connection = new Npgsql.NpgsqlConnection(_configurationRoot.GetConnectionString(IdentityDbConfig.StringConnectionName));
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to the database.");
            }
            
        }


        public int ExecuteSQL(string commandText, Dictionary<string, object> parameters)
        {
            lock (_consulta)
            {
                var result = 0;

                if (string.IsNullOrEmpty(commandText))
                {
                    throw new ArgumentException("Command text cannot be null or empty.");
                }

                try
                {
                    OpenConnection();
                    var command = CreateCommand(commandText, parameters);
                    result = command.ExecuteNonQuery();
                }
                finally
                {

                }

                return result;
            }
        }


        public object ExecuteQueryGetSingleObject(string commandText, Dictionary<string, object> parameters)
        {
            lock (_consulta)
            {
                object result = null;

                if (string.IsNullOrEmpty(commandText))
                {
                    throw new ArgumentException("Command text cannot be null or empty.");
                }

                try
                {
                    OpenConnection();
                    var command = CreateCommand(commandText, parameters);
                    result = command.ExecuteScalar();
                }
                finally
                {
                    CloseConnection();
                }

                return result;
            }
        }

        public Dictionary<string, string> ExecuteQueryGetSingleRow(string commandText,
            Dictionary<string, object> parameters)
        {
            lock (_consulta)
            {
                Dictionary<string, string> row = null;
                if (string.IsNullOrEmpty(commandText))
                {
                    throw new ArgumentException("Command text cannot be null or empty.");
                }

                try
                {
                    OpenConnection();
                    var command = CreateCommand(commandText, parameters);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            row = new Dictionary<string, string>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                                row.Add(columnName, columnValue);
                            }
                            break;
                        }
                    }
                }
                finally
                {
                    CloseConnection();
                }

                return row;
            }
        }

        public List<Dictionary<string, string>> ExecuteQuery(string commandText, Dictionary<string, object> parameters)
        {
            lock (_consulta)
            {
                List<Dictionary<string, string>> rows = null;
                if (string.IsNullOrEmpty(commandText))
                {
                    throw new ArgumentException("Command text cannot be null or empty.");
                }

                try
                {
                    OpenConnection();
                    var command = CreateCommand(commandText, parameters);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        rows = new List<Dictionary<string, string>>();
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, string>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                                row.Add(columnName, columnValue);
                            }
                            rows.Add(row);
                        }
                    }
                }
                finally
                {
                    CloseConnection();
                }

                return rows;
            }
        }

        private NpgsqlCommand CreateCommand(string commandText, Dictionary<string, object> parameters)
        {

            NpgsqlCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);

            return command;

        }

        /// <summary>
        ///     Adds the parameters to a PostgreSQL command.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        private static void AddParameters(NpgsqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var param in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                parameter.Value = param.Value ?? DBNull.Value;

                var s = param.Value as string;
                if ((s != null) && s.StartsWith("JSON"))
                {
                    parameter.Value = JObject.Parse(s.Replace("JSON", ""));
                    parameter.NpgsqlDbType = NpgsqlDbType.Json;
                }

                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        ///     Helper method to return query a string value.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        /// <returns>The string value resulting from the query.</returns>
        private string GetStrValue(string commandText, Dictionary<string, object> parameters)
        {
            var value = ExecuteQueryGetSingleObject(commandText, parameters) as string;
            return value;
        }

        /// <summary>
        ///     Opens a connection if not open.
        /// </summary>
        public void OpenConnection()
        {

            if (_connection == null)
            {
                if (string.IsNullOrEmpty(IdentityDbConfig.StringConnectionName))
                    IdentityDbConfig.StringConnectionName = "PostgreSQLBaseConnection";

                _connection =
                    new Npgsql.NpgsqlConnection(_configurationRoot.GetConnectionString(IdentityDbConfig.StringConnectionName));
            }
            var retries = 20;
            if (_connection.State == ConnectionState.Open)
            {
            }
            else
            {
                while (retries >= 0 && _connection.State != ConnectionState.Open)
                {
                    lock (_consulta)
                    {
                        _connection.Close();
                        _connection.Open();
                    }
                    retries--;
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        ///     Closes the connection if it is open.
        /// </summary>
        public void CloseConnection()
        {
            /*   if (_connection.State == ConnectionState.Open)
               {
              //     _connection.Close();
               }*/
        }

        public void Dispose()
        {

        }
    }
}