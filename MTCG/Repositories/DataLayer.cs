using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace MTCG.Repositories
{
    public static class DataLayer
    {
        // Connection to the database
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(
                "Server=localhost;" +
                "Username=postgres;" +
                "Password=postgres;" +
                "Database=mtcgdb"
                );
        }
        // Add a parameter to a command to prevent SQL injection
        public static void AddParameter(NpgsqlCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            if (value != null)
            {
                parameter.NpgsqlDbType = GetDbType(value);
            }

            command.Parameters.Add(parameter);
        }
        // could be put in to helpers
        private static NpgsqlDbType GetDbType(object value)
        {
            return value switch
            {
                int => NpgsqlDbType.Integer,
                long => NpgsqlDbType.Bigint,
                string => NpgsqlDbType.Text,
                DateTime => NpgsqlDbType.Timestamp,
                bool => NpgsqlDbType.Boolean,
                Guid => NpgsqlDbType.Uuid,
                float => NpgsqlDbType.Real,
                double => NpgsqlDbType.Double,
                _ => throw new InvalidOperationException($"Unsupported parameter type: {value.GetType()}")
            };
        }
    }
}
