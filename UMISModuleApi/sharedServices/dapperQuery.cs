
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using UMISModuleAPI.Configuration;
using UMISModuleAPI.Entities;
using Npgsql;

namespace UMISModuleAPI.Services
{
    public class dapperQuery
    {
        public static IEnumerable<T> Qry<T>(string sql, IOptions<conStr> conStr)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(conStr.Value.dbCon))
            {
                return con.Query<T>(sql);
            }
        }
        // public static IEnumerable<string> SPReturn<T>(string procName, T model, IOptions<conStr> conStr)
        // {
        //     using (NpgsqlConnection con = new NpgsqlConnection(conStr))
        //     {
        //         var row = con.Query<string>(procName, model, commandType: CommandType.StoredProcedure);

        //         return row;
        //     }
        // }
    }
}