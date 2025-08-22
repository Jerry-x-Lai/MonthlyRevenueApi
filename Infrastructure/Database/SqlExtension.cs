using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.Extensions.Logging;

namespace MonthlyRevenueApi.Infrastructure.Database
{
    /// <summary>透過資料庫操做 storeprocedure 達成資料讀寫與取用的實作</summary>
    public class SqlExtension : ISqlExtension
    {
        /// <summary>紀錄 logger </summary>
        private readonly ILogger<SqlExtension> _logger;

        /// <summary>建立資料庫連線工廠</summary>
        private readonly IDbConnectionFactory _dbConnectionFactory;

        /// <summary>建構式</summary>
        /// <param name="logger">log 紀錄器</param>
        /// <param name="dbConnectionFactory">取資料庫連線的工廠類別</param>
        public SqlExtension(ILogger<SqlExtension> logger, IDbConnectionFactory dbConnectionFactory)
        {
            _logger = logger;
            _dbConnectionFactory = dbConnectionFactory;
        }

        /// <summary> 執行 Stored Procedure </summary>
        public SqlReturn Execute(SqlPropety sqlProperty, string dbErrorCode, Func<IEnumerable<SqlParamter>> callback, string methodName = null, [CallerLineNumber] int line = 0)
        {
            var parameterResult = new Dictionary<string, object>();
            try
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    var stackTrace = new StackTrace();
                    var methodBase = stackTrace.GetFrame(0)?.GetMethod();
                    methodName = methodBase.ReflectedType.Name + "." + methodBase.Name;
                }
                var spConn = sqlProperty.DbConnectionName;
                var sp = $"[dbo].[{sqlProperty.SpName}]";
                var parameters = callback()?.ToList() ?? new List<SqlParamter>();
                var dp = new DynamicParameters();

                _logger.LogInformation($"Execute {sp} | Params: {string.Join(",", parameters.Select(p => p.Name))}");

                using (var conn = _dbConnectionFactory.CreateDbConnection(spConn))
                {
                    if (parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            if (item.Direction == SqlParamDirectionEnum.InputOutput || item.Direction == SqlParamDirectionEnum.Output)
                                dp.Add(item.Name, item.Value, direction: (ParameterDirection)item.Direction, size: int.MaxValue);
                            else
                            {
                                if (item.IsDataTable)
                                    dp.Add(item.Name, item.Value, DbType.Object);
                                else
                                    dp.Add(item.Name, item.Value);
                            }
                        }
                    }
                    conn.Query(sp, dp, commandType: CommandType.StoredProcedure);
                    conn.Close();
                    conn.Dispose();
                }
                if (parameters.Count > 0)
                {
                    parameters
                        .Where(x => x.Direction is SqlParamDirectionEnum.InputOutput or SqlParamDirectionEnum.Output)
                        .ToList()
                        .ForEach(x => parameterResult.Add(x.Name, dp.Get<object>(x.Name)));
                }
                var sqlResult = new SqlReturn() { ParameterResult = parameterResult };
                if (parameterResult.Any(x => x.Key.IndexOf("outstring", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    var outStringItem = parameterResult.FirstOrDefault(x =>
                        x.Key.Contains("outstring", StringComparison.OrdinalIgnoreCase));
                    sqlResult.OutString = outStringItem.Value?.ToString();
                }
                return sqlResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Execute發生錯誤 | {sqlProperty.SpName} | 錯誤代碼={dbErrorCode}");
                parameterResult.Add("@CatchErrMsg", ex.Message);
                return new SqlReturn() { OutString = dbErrorCode, ParameterResult = parameterResult };
            }
        }

        /// <summary> 執行 Stored Procedure </summary>
        public SqlReturn<T> Execute<T>(SqlPropety sqlProperty, string dbErrorCode, Func<IEnumerable<SqlParamter>> callback, string methodName = null, [CallerLineNumber] int line = 0)
        {
            var parameterResult = new Dictionary<string, object>();
            var queryResult = default(List<T>);
            try
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    var stackTrace = new StackTrace();
                    var methodBase = stackTrace.GetFrame(0)?.GetMethod();
                    methodName = methodBase.ReflectedType.Name + "." + methodBase.Name;
                }
                var spConn = sqlProperty.DbConnectionName;
                var sp = $"[dbo].[{sqlProperty.SpName}]";
                var parameters = callback()?.ToList() ?? new List<SqlParamter>();
                var dp = new DynamicParameters();

                using (var conn = _dbConnectionFactory.CreateDbConnection(spConn))
                {
                    if (parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            if (item.Direction == SqlParamDirectionEnum.InputOutput || item.Direction == SqlParamDirectionEnum.Output)
                                dp.Add(item.Name, item.Value, direction: (ParameterDirection)item.Direction, size: int.MaxValue);
                            else
                            {
                                if (item.IsDataTable)
                                    dp.Add(item.Name, item.Value, DbType.Object);
                                else
                                    dp.Add(item.Name, item.Value);
                            }
                        }
                    }
                    queryResult = conn.Query<T>(sp, dp, commandType: CommandType.StoredProcedure).ToList();
                    conn.Close();
                    conn.Dispose();
                }
                if (parameters.Count > 0)
                {
                    parameters
                        .Where(x => x.Direction is SqlParamDirectionEnum.InputOutput or SqlParamDirectionEnum.Output)
                        .ToList()
                        .ForEach(x => parameterResult.Add(x.Name, dp.Get<object>(x.Name)));
                }
                var sqlResult = new SqlReturn<T>() { Result = queryResult, ParameterResult = parameterResult };
                if (parameterResult.Any(x => x.Key.Contains("outstring", StringComparison.OrdinalIgnoreCase)))
                {
                    var outStringItem = parameterResult.FirstOrDefault(x =>
                        x.Key.Contains("outstring", StringComparison.OrdinalIgnoreCase));
                    sqlResult.OutString = outStringItem.Value?.ToString();
                }
                return sqlResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Execute發生錯誤 | {sqlProperty.SpName} | 錯誤代碼={dbErrorCode}");
                parameterResult.Add("@strOutstring", dbErrorCode);
                return new SqlReturn<T>() { OutString = dbErrorCode, Result = new List<T>(), ParameterResult = parameterResult };
            }
        }

        /// <summary> 執行 Stored Procedure </summary>
        public SqlReturn<T1, T2> Execute<T1, T2>(SqlPropety sqlProperty, string dbErrorCode, Func<IEnumerable<SqlParamter>> parameterFunc, string methodName = null, [CallerLineNumber] int line = 0)
        {
            var parameterResult = new Dictionary<string, object>();
            var result1 = default(List<T1>);
            var result2 = default(List<T2>);
            try
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    var stackTrace = new StackTrace();
                    var methodBase = stackTrace.GetFrame(0)?.GetMethod();
                    methodName = methodBase.ReflectedType.Name + "." + methodBase.Name;
                }
                var spConn = sqlProperty.DbConnectionName;
                var sp = $"[dbo].[{sqlProperty.SpName}]";
                var parameters = parameterFunc()?.ToList() ?? new List<SqlParamter>();
                var dp = new DynamicParameters();

                using (var conn = _dbConnectionFactory.CreateDbConnection(spConn))
                {
                    if (parameters.Count > 0)
                    {
                        foreach (var item in parameters)
                        {
                            if (item.Direction == SqlParamDirectionEnum.InputOutput || item.Direction == SqlParamDirectionEnum.Output)
                                dp.Add(item.Name, item.Value, direction: (ParameterDirection)item.Direction, size: int.MaxValue);
                            else
                            {
                                if (item.IsDataTable)
                                    dp.Add(item.Name, item.Value, DbType.Object);
                                else
                                    dp.Add(item.Name, item.Value);
                            }
                        }
                    }
                    using (var result = conn.QueryMultiple(sp, dp, commandType: CommandType.StoredProcedure))
                    {
                        if (!result.IsConsumed)
                        {
                            result1 = result.Read<T1>().ToList();
                        }
                        if (!result.IsConsumed)
                        {
                            result2 = result.Read<T2>().ToList();
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
                if (parameters.Count > 0)
                {
                    parameters
                        .Where(x => x.Direction is SqlParamDirectionEnum.InputOutput or SqlParamDirectionEnum.Output)
                        .ToList()
                        .ForEach(x => parameterResult.Add(x.Name, dp.Get<object>(x.Name)));
                }
                var sqlResult = new SqlReturn<T1, T2>
                {
                    T1Result = result1 ?? new List<T1>(),
                    T2Result = result2 ?? new List<T2>(),
                    ParameterResult = parameterResult
                };
                if (parameterResult.Any(x => x.Key.Contains("outstring", StringComparison.OrdinalIgnoreCase)))
                {
                    var outStringItem = parameterResult.FirstOrDefault(x =>
                        x.Key.Contains("outstring", StringComparison.OrdinalIgnoreCase));
                    sqlResult.OutString = outStringItem.Value?.ToString();
                }
                return sqlResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Execute發生錯誤 | {sqlProperty.SpName} | 錯誤代碼={dbErrorCode}");
                parameterResult.Add("@strOutstring", dbErrorCode);
                return new SqlReturn<T1, T2> { OutString = dbErrorCode, T1Result = new List<T1>(), T2Result = new List<T2>(), ParameterResult = parameterResult };
            }
        }
    }
}