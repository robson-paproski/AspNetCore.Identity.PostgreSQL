using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace AspNetIdentity3PostgreSQL
{
    /// <summary>
    /// Class that encapsulates PostgreSQL database connections and CRUD operations.
    /// </summary>
    public class PostgreSQLDatabase : IDisposable
    {
        private Npgsql.NpgsqlConnection _connection;

        /// Default constructor which uses the "DefaultConnection" connectionString, often located in web.config.
        /// </summary>
        public PostgreSQLDatabase()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        /// Constructor which takes the connection string name.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection string.</param>
        public PostgreSQLDatabase(string connectionStringName)
        {
            var configbuilder =
                new ConfigurationBuilder()
                    .AddJsonFile("config.json");
            var config = configbuilder.Build();
            var connectionString = config.Get(connectionStringName);
            _connection = new Npgsql.NpgsqlConnection(connectionString);
        }

        public int ExecuteSQL(string commandText, Dictionary<string, object> parameters)
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
                _connection.Close();
            }

            return result;
        }


        public object ExecuteQueryGetSingleObject(string commandText, Dictionary<string, object> parameters)
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

        public Dictionary<string, string> ExecuteQueryGetSingleRow(string commandText,
            Dictionary<string, object> parameters)
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
                using (var reader = command.ExecuteReader())
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

        public List<Dictionary<string, string>> ExecuteQuery(string commandText, Dictionary<string, object> parameters)
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
                using (var reader = command.ExecuteReader())
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

        private Npgsql.NpgsqlCommand CreateCommand(string commandText, Dictionary<string, object> parameters)
        {
            Npgsql.NpgsqlCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);

            return command;
        }

        /// <summary>
        ///     Adds the parameters to a PostgreSQL command.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        private static void AddParameters(Npgsql.NpgsqlCommand command, Dictionary<string, object> parameters)
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
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        ///     Helper method to return query a string value.
        /// </summary>
        /// <param name="commandText">The PostgreSQL query to execute.</param>
        /// <param name="parameters">Parameters to pass to the PostgreSQL query.</param>
        /// <returns>The string value resulting from the query.</returns>
        public string GetStrValue(string commandText, Dictionary<string, object> parameters)
        {
            var value = ExecuteQueryGetSingleObject(commandText, parameters) as string;
            return value;
        }

        /// <summary>
        ///     Opens a connection if not open.
        /// </summary>
        public void OpenConnection()
        {
            var retries = 10;
            if (_connection.State == ConnectionState.Open)
            {
            }
            else
            {
                while (retries >= 0 && _connection.State != ConnectionState.Open)
                {
                    _connection.Open();
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
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}