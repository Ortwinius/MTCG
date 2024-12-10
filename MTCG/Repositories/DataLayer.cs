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
                "Server=localhost;Username=postgres;Password=postgres;Database=mtcgdb"
                );
        }
        // Add a parameter to a command to prevent SQL injection
        public static void AddParameterWithValue(NpgsqlCommand command, string parameterName, NpgsqlDbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.NpgsqlDbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

    }
}
