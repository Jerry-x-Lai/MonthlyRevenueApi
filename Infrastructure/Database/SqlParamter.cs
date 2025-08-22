using System.Data;

namespace MonthlyRevenueApi.Infrastructure.Database
{
    public class SqlParamter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SqlParamDirectionEnum Direction { get; set; } = SqlParamDirectionEnum.Input;
        public bool IsDataTable { get; set; } = false;

        public SqlParamter(string name, object value, SqlParamDirectionEnum direction = SqlParamDirectionEnum.Input)
        {
            Name = name;
            Value = value;
            Direction = direction;
            IsDataTable = false;
        }

        public SqlParamter(string name, object value, bool isDataTable)
        {
            Name = name;
            Value = value;
            Direction = SqlParamDirectionEnum.Input;
            IsDataTable = isDataTable;
        }
    }

    public enum SqlParamDirectionEnum
    {
        Input = ParameterDirection.Input,
        Output = ParameterDirection.Output,
        InputOutput = ParameterDirection.InputOutput
    }
}
