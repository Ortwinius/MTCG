using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG.Repositories
{
    public static class DataLayer
    {
        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

    }
}
